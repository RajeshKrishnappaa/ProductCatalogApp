import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Nav from "./Nav";
import ProductContext from "../context/ProductContext";

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

const renderNav = () => {
  return render(
    <ProductContext.Provider value={mockContext}>
      <Nav showfilters={true} />
    </ProductContext.Provider>
  );
};

describe("Nav Component", () => {

  test("renders search input", () => {
    renderNav();
    expect(screen.getByPlaceholderText("Search products...")).toBeInTheDocument();
  });

  test("typing in search calls setSearch", async () => {
    const user = userEvent.setup();
    renderNav();

    const input = screen.getByPlaceholderText("Search products...");
    await user.type(input, "laptop");

    expect(mockContext.setSearch).toHaveBeenCalled();
  });

  test("renders category dropdown", () => {
    renderNav();

    expect(screen.getByText("Electronics")).toBeInTheDocument();
    expect(screen.getByText("Clothing")).toBeInTheDocument();
  });

  test("category change calls setCategoryId", async () => {
    const user = userEvent.setup();
    renderNav();

    const selects = screen.getAllByRole("combobox");
    const categorySelect = selects[0];

    await user.selectOptions(categorySelect, "1");

    expect(mockContext.setCategoryId).toHaveBeenCalledWith("1");
  });

  test("price range change calls setPriceRange", async () => {
    const user = userEvent.setup();
    renderNav();
    const selects = screen.getAllByRole("combobox");
    const priceDropdown = selects[1];

    await user.selectOptions(priceDropdown, "5000+");

    expect(mockContext.setPriceRange).toHaveBeenCalledWith([5000, 100000]);
  });

});
