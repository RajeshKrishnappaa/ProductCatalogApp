import { render, screen, fireEvent } from "@testing-library/react";
import Signup from "./Signup";
import { ProductProvider } from "../context/ProductContext";
import api from "../api/api";

jest.mock("../api/api");
jest.mock("react-router-dom", () => ({
  useNavigate: () => jest.fn(),
  Link: ({ children }) => <a>{children}</a>
}));

beforeAll(() => {
  window.alert = jest.fn();
});

test("Signup submits form and calls signup API", async () => {
  api.post.mockResolvedValueOnce({ data: {} });

  render(
    <ProductProvider>
      <Signup />
    </ProductProvider>
  );

  const inputs = screen.getAllByRole("textbox");
  fireEvent.change(inputs[0], { target: { value: "rajesh" } }); // username
  fireEvent.change(inputs[1], { target: { value: "rajesh@test.com" } }); // email

  const passwordInput = screen.getByLabelText(/password/i, { selector: "input" });
  fireEvent.change(passwordInput, { target: { value: "123456" } });

  fireEvent.click(screen.getByRole("button", { name: /signup/i }));

  expect(api.post).toHaveBeenCalledWith("/User/Register", {
    userName: "rajesh",
    email: "rajesh@test.com",
    password: "123456",
  });
});
