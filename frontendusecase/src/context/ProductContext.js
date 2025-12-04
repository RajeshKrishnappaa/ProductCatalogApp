import { createContext, useState, useEffect, useCallback } from "react";
import api from "../api/api";

const ProductContext = createContext();

export const ProductProvider = ({ children }) => {

  const [user, setUser] = useState(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");
    const username = localStorage.getItem("username");

    return token ? { token, role, username } : null;
  });

  const [authError, setAuthError] = useState("");

  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);

  const [search, setSearch] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [priceRange, setPriceRange] = useState([0, 100000]);

  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(5);
  const [totalProducts, setTotalProducts] = useState(0);

  const loadCategories = useCallback(async () => {
    try {
      const res = await api.get("/Category/All");
      setCategories(res.data);
    } catch (err) {
      console.error("Category load failed", err);
    }
  }, []);

  const loadProducts = useCallback(async () => {
    try {
      const res = await api.get("/Product/All", {
        params: {
          page: currentPage,
          pageSize,
          q: search || null,
          categoryId: categoryId || null,
          minPrice: priceRange[0] === "" ? null : Number(priceRange[0]),
          maxPrice: priceRange[1] === "" ? null : Number(priceRange[1]),
        },
      });

      setProducts(res.data.items);
      setTotalProducts(res.data.total);
    } catch (err) {
      console.error("Product load failed", err);
    }
  }, [currentPage, search, categoryId, priceRange, pageSize]);

  useEffect(() => {
    loadProducts();
  }, [loadProducts, currentPage, search, categoryId, priceRange]);

  useEffect(() => {
    loadCategories();
  }, [loadCategories]);

  const login = async (username, password) => {
    try {
      const res = await api.post("/Login/Login", {
        userName: username,
        password: password,
      });

      localStorage.setItem("token", res.data.token);
      localStorage.setItem("role", res.data.role);
      localStorage.setItem("username", res.data.username);

      setUser({
        token: res.data.token,
        role: res.data.role,
        username: res.data.userName,
      });

      loadProducts();
      setAuthError("");
      return { ok: true, user: res.data };
    } catch (err) {
      setAuthError("Invalid username or password");
      return { ok: false };
    }
  };

  const signup = async (username, email, password) => {
    try {
      await api.post("/User/Register", {
        userName: username,
        email,
        password,
      });

      setAuthError("");
      return { ok: true };
    } catch (err) {
      if (err.response && err.response.data) {
        setAuthError(err.response.data);
      } else {
        setAuthError("Signup failed");
      }
      return { ok: false };
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    localStorage.removeItem("username");
    setUser(null);

    setSearch("");
    setCategoryId("");
    setPriceRange([0, 100000]);
    setCurrentPage(1);
    setProducts([]);
  };

  const deleteProduct = async (id) => {
    await api.delete(`/Product/Delete/${id}`);
    loadProducts();
  };

  return (
    <ProductContext.Provider
      value={{
        user,
        setUser,
        authError,
        setAuthError,
        products,
        setProducts,
        categories,
        setCategories,
        search,
        setSearch,
        categoryId,
        setCategoryId,
        priceRange,
        setPriceRange,
        currentPage,
        setCurrentPage,
        totalProducts,
        setTotalProducts,
        pageSize,
        loadCategories,
        loadProducts,
        login,
        signup,
        logout,
        deleteProduct,
      }}
    >
      {children}
    </ProductContext.Provider>
  );
};

export default ProductContext;
