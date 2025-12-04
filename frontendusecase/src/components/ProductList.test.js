// src/components/ProductList.test.js
import { render, screen } from "@testing-library/react";
import ProductList from "./ProductList";
import ProductContext from "../context/ProductContext";

jest.mock("./ProductCard", () => (props) => (
  <div data-testid="product-card">{props.product.productName}</div>
));

jest.mock("./Pagination", () => (props) => (
  <div data-testid="pagination">Pagination</div>
));

describe("ProductList Component", () => {
  test("shows empty message when no products", () => {
    const ctx = {
      products: [],
      totalProducts: 0,
      currentPage: 1,
      pageSize: 10,
      setCurrentPage: jest.fn(),
    };

    render(
      <ProductContext.Provider value={ctx}>
        <ProductList />
      </ProductContext.Provider>
    );

    expect(screen.getByText("No products found.")).toBeInTheDocument();
  });

  test("renders product cards", () => {
    const ctx = {
      products: [
        { productId: 1, productName: "Laptop" },
        { productId: 2, productName: "Phone" },
      ],
      totalProducts: 2,
      currentPage: 1,
      pageSize: 10,
      setCurrentPage: jest.fn(),
    };

    render(
      <ProductContext.Provider value={ctx}>
        <ProductList />
      </ProductContext.Provider>
    );

    expect(screen.getAllByTestId("product-card")).toHaveLength(2);
  });
});
