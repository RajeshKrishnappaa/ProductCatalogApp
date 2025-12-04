// src/components/ProductCard.test.js
import { render, screen, fireEvent } from "@testing-library/react";
import ProductCard from "./ProductCard";
import ProductContext from "../context/ProductContext";
import { mockNavigate } from "../__mocks__/react-router-dom";

const sampleProduct = {
  productId: 1,
  productName: "Laptop",
  price: 50000,
  description: "Good laptop",
  imageUrl: "image.jpg",
};

describe("ProductCard Component", () => {
  test("renders product details", () => {
    render(
      <ProductContext.Provider value={{ user: { role: "User" } }}>
        <ProductCard product={sampleProduct} />
      </ProductContext.Provider>
    );

    expect(screen.getByText("Laptop")).toBeInTheDocument();
    expect(screen.getByText("₹ 50000")).toBeInTheDocument();
  });

  test("navigates to login when user not logged", () => {
    render(
      <ProductContext.Provider value={{ user: null }}>
        <ProductCard product={sampleProduct} />
      </ProductContext.Provider>
    );

    fireEvent.click(screen.getByText("Laptop"));
    expect(mockNavigate).toHaveBeenCalledWith("/login");
  });

  test("navigates to product page when user logged", () => {
    render(
      <ProductContext.Provider value={{ user: { role: "User" } }}>
        <ProductCard product={sampleProduct} />
      </ProductContext.Provider>
    );

    fireEvent.click(screen.getByText("Laptop"));
    expect(mockNavigate).toHaveBeenCalledWith("/product/1");
  });
});
