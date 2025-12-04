import React from "react";
import { renderHook, act } from "@testing-library/react";
import ProductContext, { ProductProvider } from "./ProductContext";
import api from "../api/api";

jest.mock("../api/api");

// Correct wrapper
const Wrapper = ({ children }) => (
  <ProductProvider>{children}</ProductProvider>
);

beforeEach(() => {
  api.get.mockResolvedValue({
    data: { items: [], total: 0 },
  });

  api.post.mockResolvedValue({ data: {} });
  api.delete.mockResolvedValue({});
});

describe("ProductContext", () => {
  test("login success updates user & clears authError", async () => {
    api.post.mockResolvedValueOnce({
      data: {
        token: "123",
        role: "Admin",
        username: "admin",
        userName: "admin",
      },
    });

    const { result } = renderHook(
      () => React.useContext(ProductContext),
      { wrapper: Wrapper }
    );

    await act(async () => {
      const res = await result.current.login("admin", "123");
      expect(res.ok).toBe(true);
    });

    expect(result.current.user.role).toBe("Admin");
    expect(result.current.authError).toBe("");
  });

  test("login failure sets authError", async () => {
    api.post.mockRejectedValueOnce(new Error("Invalid"));

    const { result } = renderHook(
      () => React.useContext(ProductContext),
      { wrapper: Wrapper }
    );

    await act(async () => {
      await result.current.login("wrong", "wrong");
    });

    expect(result.current.authError).toBe("Invalid username or password");
  });

  test("logout clears user & resets filters", () => {
    const { result } = renderHook(
      () => React.useContext(ProductContext),
      { wrapper: Wrapper }
    );

    act(() => {
      result.current.setUser({ token: "abc", role: "User", username: "test" });
      result.current.setSearch("phone");
      result.current.setCategoryId("2");
      result.current.setPriceRange([100, 500]);
    });

    act(() => {
      result.current.logout();
    });

    expect(result.current.user).toBe(null);
    expect(result.current.search).toBe("");
    expect(result.current.categoryId).toBe("");
    expect(result.current.priceRange).toEqual([0, 100000]);
  });

  test("loadCategories fetches data", async () => {
  api.get
    .mockResolvedValueOnce({ data: { items: [], total: 0 } }) // initial loadProducts()
    .mockResolvedValueOnce({
      data: [{ categoryId: 1, categoryName: "Electronics" }],
    });

  const { result } = renderHook(() => React.useContext(ProductContext), {
    wrapper: Wrapper,
  });

  // wait for initial effects to run
  await act(async () => {
    await Promise.resolve();
  });

  await act(async () => {
    await result.current.loadCategories();
  });

  expect(result.current.categories.length).toBe(1);
  expect(result.current.categories[0].categoryName).toBe("Electronics");
});

  test("loadProducts fetches paginated data", async () => {
    api.get.mockResolvedValueOnce({
      data: {
        items: [{ id: 1, name: "Laptop" }],
        total: 10,
      },
    });

    const { result } = renderHook(
      () => React.useContext(ProductContext),
      { wrapper: Wrapper }
    );

    await act(async () => {
      await result.current.loadProducts();
    });

    expect(result.current.products.length).toBe(1);
    expect(result.current.totalProducts).toBe(10);
  });
});
