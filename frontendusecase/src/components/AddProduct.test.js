import { render, screen, fireEvent } from "@testing-library/react";
import AddProduct from "./AddProduct";
import ProductContext from "../context/ProductContext";
import api from "../api/api";

jest.mock("../api/api");
jest.mock("react-router-dom", () => ({
  useNavigate: () => jest.fn()
}));

beforeAll(() => {
  window.alert = jest.fn();
});

test("submits product form and calls API correctly", async () => {
  const mockLoadCategories = jest.fn();
  const mockLoadProducts = jest.fn();
  const mockNavigate = jest.fn();

  api.post.mockResolvedValueOnce({ data: {} });

  const fakeImage = new File(["img"], "test.png", { type: "image/png" });

  render(
    <ProductContext.Provider
      value={{
        user: { role: "Admin" },
        categories: [{ categoryId: 1, categoryName: "Electronics" }],
        loadCategories: mockLoadCategories,
        loadProducts: mockLoadProducts
      }}
    >
      <AddProduct />
    </ProductContext.Provider>
  );

  const inputs = screen.getAllByRole("textbox");
  fireEvent.change(inputs[0], { target: { value: "Laptop" } });
  fireEvent.change(inputs[1], { target: { value: "New Laptop" } });

  fireEvent.change(screen.getByLabelText(/price/i), {
    target: { value: "1500" }
  });

  fireEvent.change(screen.getByRole("combobox"), {
    target: { value: "1" }
  });

  fireEvent.change(screen.getByLabelText(/upload image/i), {
    target: { files: [fakeImage] }
  });

  fireEvent.click(screen.getByRole("button", { name: /add product/i }));

  expect(api.post).toHaveBeenCalled();
  const formDataSent = api.post.mock.calls[0][1];
  expect(formDataSent.get("ProductName")).toBe("Laptop");
  expect(formDataSent.get("Description")).toBe("New Laptop");
  expect(formDataSent.get("Price")).toBe("1500");
  expect(formDataSent.get("CategoryId")).toBe("1");
  expect(formDataSent.get("ImageFile")).toBe(fakeImage);

  expect(mockLoadProducts).toHaveBeenCalled();
});
