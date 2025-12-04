import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import AddProduct from "./AddProduct";
import ProductContext from "../context/ProductContext";
import api from "../api/api";

// Mock API
jest.mock("../api/api", () => ({
  post: jest.fn(),
}));

// Mock navigation
const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useNavigate: () => mockNavigate,
}));

describe("AddProduct Component", () => {
  test("submits product form and calls API + loadProducts", async () => {
    const mockLoadProducts = jest.fn();

    const contextValues = {
      categories: [{ categoryId: 1, categoryName: "Electronics" }],
      loadProducts: mockLoadProducts,
    };

    render(
      <ProductContext.Provider value={contextValues}>
        <AddProduct />
      </ProductContext.Provider>
    );

    fireEvent.change(screen.getByTestId("name-input"), {
      target: { value: "Laptop" },
    });

    fireEvent.change(screen.getByTestId("price-input"), {
      target: { value: "50000" },
    });

    fireEvent.change(screen.getByTestId("desc-input"), {
      target: { value: "Good laptop" },
    });

    fireEvent.change(screen.getByTestId("category-select"), {
      target: { value: "1" },
    });

    fireEvent.submit(screen.getByTestId("add-form"));

    await waitFor(() => {
      expect(api.post).toHaveBeenCalledTimes(1);

      const callArg = api.post.mock.calls[0];
      expect(callArg[0]).toBe("/Product/Add");

      expect(mockLoadProducts).toHaveBeenCalled(); // FIXED
      expect(mockNavigate).toHaveBeenCalledWith("/admin");
    });
  });
});
