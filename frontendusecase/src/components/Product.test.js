import { render, screen, fireEvent } from "@testing-library/react";
import ProductList from "./ProductList";
import ProductContext from "../context/ProductContext";

jest.mock("./ProductCard", () => {
  return ({ product }) => <div data-testid="product-card">{product.productName}</div>;
});

describe("ProductList Component", () => {
  const mockSetPage = jest.fn();

  const contextValues = {
    products: [
      { productId: 1, productName: "Laptop", price: 50000 },
      { productId: 2, productName: "Mobile", price: 20000 },
    ],
    totalProducts: 20,
    currentPage: 1,
    pageSize: 10,
    setCurrentPage: mockSetPage,
  };

  const renderList = (values = contextValues) => {
    return render(
      <ProductContext.Provider value={values}>
        <ProductList />
      </ProductContext.Provider>
    );
  };

  test("shows 'No products found' when list is empty", () => {
    renderList({
      ...contextValues,
      products: [],
    });

    expect(screen.getByText("No products found.")).toBeInTheDocument();
  });

  test("renders product cards", () => {
    renderList();

    const cards = screen.getAllByTestId("product-card");
    expect(cards.length).toBe(2);
    expect(cards[0].textContent).toBe("Laptop");
  });

  test("pagination triggers page change", () => {
    renderList();

    const nextBtn = screen.getByRole("button", { name: /next/i });

    fireEvent.click(nextBtn);

    expect(mockSetPage).toHaveBeenCalledWith(2);
  });
});
