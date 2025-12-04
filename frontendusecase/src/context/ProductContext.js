// src/context/ProductContext.js
import React, { createContext, useState, useEffect, useCallback } from "react";
import api from "../api/api";

const ProductContext = createContext({
  // default values for safety (so tests/components that import context directly don't blow up)
  user: null,
  setUser: () => {},
  login: async () => ({ ok: false }),
  signup: async () => ({ ok: false }),
  logout: () => {},
  categories: [],
  products: [],
  totalProducts: 0,
  loadCategories: async () => {},
  loadProducts: async () => {},
  deleteProduct: async () => {},
  search: "",
  setSearch: () => {},
  categoryId: "",
  setCategoryId: () => {},
  priceRange: [0, 100000],
  setPriceRange: () => {},
  currentPage: 1,
  setCurrentPage: () => {},
  pageSize: 10,
  setPageSize: () => {},
  authError: "",
  setAuthError: () => {},
});

export const ProductProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");
    const username = localStorage.getItem("username");
    return token ? { token, role, username } : null;
  });

  const [categories, setCategories] = useState([]);
  const [products, setProducts] = useState([]);
  const [totalProducts, setTotalProducts] = useState(0);

  const [search, setSearch] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [priceRange, setPriceRange] = useState([0, 100000]);

  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const [authError, setAuthError] = useState("");

  // load categories
  const loadCategories = useCallback(async () => {
    try {
      const res = await api.get("/Category/All");
      // defensive: ensure array
      setCategories(res?.data ?? []);
    } catch (err) {
      console.error("Category load failed", err);
      setCategories([]);
    }
  }, []);

  // load products (paginated/filter support minimal but included)
  const loadProducts = useCallback(
    async (page = currentPage, size = pageSize) => {
      try {
        // build minimal params based on state
        const params = {
          page,
          pageSize: size,
          search,
          categoryId,
          minPrice: priceRange?.[0],
          maxPrice: priceRange?.[1],
        };

        const res = await api.get("/Product/GetPaged", { params });
        // defensive fallback
        setProducts(res?.data?.items ?? []);
        setTotalProducts(res?.data?.total ?? 0);
      } catch (err) {
        console.error("Product load failed", err);
        setProducts([]);
        setTotalProducts(0);
      }
    },
    [currentPage, pageSize, search, categoryId, priceRange]
  );

  const deleteProduct = useCallback(
    async (productId) => {
      try {
        await api.delete(`/Product/${productId}`);
        // refresh list if possible
        await loadProducts();
      } catch (err) {
        console.error("Delete product failed", err);
      }
    },
    [loadProducts]
  );

  // auth actions
  const login = useCallback(async (username, password) => {
    try {
      const res = await api.post("/User/Login", { username, password });
      if (res?.data?.token) {
        const token = res.data.token;
        const role = res.data.role;
        const uname = res.data.username || res.data.userName || username;
        localStorage.setItem("token", token);
        localStorage.setItem("role", role);
        localStorage.setItem("username", uname);
        setUser({ token, role, username: uname });
        setAuthError("");
        return { ok: true };
      }
      setAuthError("Login failed");
      return { ok: false };
    } catch (err) {
      setAuthError(
        err?.response?.data?.message || "Invalid username or password"
      );
      return { ok: false };
    }
  }, []);

  const signup = useCallback(async (userName, email, password) => {
    try {
      await api.post("/User/Register", { userName, email, password });
      setAuthError("");
      return { ok: true };
    } catch (err) {
      setAuthError(err?.response?.data?.message || "Signup failed");
      return { ok: false };
    }
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    localStorage.removeItem("username");
    setUser(null);
    // reset filters
    setSearch("");
    setCategoryId("");
    setPriceRange([0, 100000]);
  }, []);

  // Only auto-load on real app runs. In tests we often want to call loads explicitly to avoid act() warnings.
  useEffect(() => {
    if (process.env.NODE_ENV !== "test") {
      loadCategories();
      loadProducts();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const value = {
    user,
    setUser,
    login,
    signup,
    logout,
    categories,
    products,
    totalProducts,
    loadCategories,
    loadProducts,
    deleteProduct,
    search,
    setSearch,
    categoryId,
    setCategoryId,
    priceRange,
    setPriceRange,
    currentPage,
    setCurrentPage,
    pageSize,
    setPageSize,
    authError,
    setAuthError,
  };

  return (
    <ProductContext.Provider value={value}>{children}</ProductContext.Provider>
  );
};

export default ProductContext;
