// src/components/Nav.test.js
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Nav from "./Nav";
import ProductContext from "../context/ProductContext";

// Mock context values
const mockContext = {
  search: "",
  setSearch: jest.fn(),
  categories: [
    { categoryId: 1, categoryName: "Electronics" },
    { categoryId: 2, categoryName: "Clothing" },
  ],
  categoryId: "",
  setCategoryId: jest.fn(),
  priceRange: [0, 100000],
  setPriceRange: jest.fn(),
};

const renderNav = () =>
  render(
    <ProductContext.Provider value={mockContext}>
      <Nav showfilters={true} />
    </ProductContext.Provider>
  );

describe("Nav Component", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("renders search input", () => {
    renderNav();
    expect(
      screen.getByPlaceholderText(/search products/i)
    ).toBeInTheDocument();
  });

  test("typing in search calls setSearch", async () => {
    const user = userEvent.setup();
    renderNav();

    const input = screen.getByPlaceholderText(/search products/i);
    await user.type(input, "laptop");

    expect(mockContext.setSearch).toHaveBeenCalledTimes(6); // l a p t o p
  });

  test("renders category dropdown items", () => {
    renderNav();
    expect(screen.getByText("Electronics")).toBeInTheDocument();
    expect(screen.getByText("Clothing")).toBeInTheDocument();
  });

  test("category change calls setCategoryId", async () => {
    const user = userEvent.setup();
    renderNav();

    const selects = screen.getAllByRole("combobox");
    const categorySelect = selects[0]; // First dropdown = Category

    await user.selectOptions(categorySelect, "1");

    expect(mockContext.setCategoryId).toHaveBeenCalledWith("1");
  });

  test("price change calls setPriceRange", async () => {
    const user = userEvent.setup();
    renderNav();

    const selects = screen.getAllByRole("combobox");
    const priceSelect = selects[1]; // Second dropdown = Price

    await user.selectOptions(priceSelect, "0-5000");

    expect(mockContext.setPriceRange).toHaveBeenCalledWith([0, 5000]);
  });
});
