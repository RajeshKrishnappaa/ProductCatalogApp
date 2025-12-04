import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import EditProduct from "./EditProduct";
import ProductContext from "../context/ProductContext";
import api from "../api/api";
import { MemoryRouter, Route, Routes } from "react-router-dom";

jest.mock("../api/api");

global.URL.createObjectURL = jest.fn(() => "preview-url");

beforeEach(() => {
  api.get.mockResolvedValue({
    data: {
      productId: 1,
      productName: "Laptop",
      description: "Fast laptop",
      price: 50000,
      categoryId: 1,
      imageUrl: "/images/laptop.png"
    }
  });

  api.put = jest.fn().mockResolvedValue({ data: {} });
});

const renderEditProduct = (contextValues) => {
  return render(
    <ProductContext.Provider value={contextValues}>
      <MemoryRouter initialEntries={["/edit/1"]}>
        <Routes>
          <Route path="/edit/:id" element={<EditProduct />} />
        </Routes>
      </MemoryRouter>
    </ProductContext.Provider>
  );
};

describe("EditProduct Component", () => {

  test("shows Unauthorized if user is not Admin", () => {
    renderEditProduct({
      user: { role: "User" },
      categories: [],
      loadCategories: jest.fn(),
      loadProducts: jest.fn(),
    });

    expect(screen.getByText(/unauthorized/i)).toBeInTheDocument();
  });

  test("loads product info on render", async () => {
    const mockLoadCategories = jest.fn();

    renderEditProduct({
      user: { role: "Admin" },
      categories: [{ categoryId: 1, categoryName: "Electronics" }],
      loadCategories: mockLoadCategories,
      loadProducts: jest.fn(),
    });

    expect(mockLoadCategories).toHaveBeenCalled();

    expect(await screen.findByDisplayValue("Laptop")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Fast laptop")).toBeInTheDocument();
    expect(screen.getByDisplayValue("50000")).toBeInTheDocument();
  });

  test("updates product and calls API", async () => {
    const mockLoadProducts = jest.fn();

    renderEditProduct({
      user: { role: "Admin" },
      categories: [{ categoryId: 1, categoryName: "Electronics" }],
      loadCategories: jest.fn(),
      loadProducts: mockLoadProducts,
    });

    fireEvent.change(screen.getByLabelText(/product name/i), {
      target: { value: "Updated Laptop" },
    });

    fireEvent.click(screen.getByRole("button", { name: /update product/i }));

    await waitFor(() => {
      expect(api.put).toHaveBeenCalled();
    });

    expect(mockLoadProducts).toHaveBeenCalled();
  });

  test("updates preview when new image is selected", async () => {
    renderEditProduct({
      user: { role: "Admin" },
      categories: [{ categoryId: 1, categoryName: "Electronics" }],
      loadCategories: jest.fn(),
      loadProducts: jest.fn(),
    });

    const file = new File(["dummy"], "image.png", { type: "image/png" });

    fireEvent.change(screen.getByLabelText(/replace image/i), {
      target: { files: [file] },
    });

    expect(URL.createObjectURL).toHaveBeenCalledWith(file);
  });
});
