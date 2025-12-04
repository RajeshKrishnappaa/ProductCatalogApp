
import { render, screen, fireEvent } from "@testing-library/react";
import Signup from "./Signup";
import { ProductProvider } from "../context/ProductContext";
import api from "../api/api";

jest.mock("../api/api");

beforeAll(() => {
  window.alert = jest.fn();
  window.confirm = jest.fn().mockReturnValue(true);
});

afterAll(() => {
  window.alert = undefined;
  window.confirm = undefined;
});

test("Signup submits form and calls signup API", async () => {
  api.post.mockResolvedValueOnce({ data: {} });

  render(
    <ProductProvider>
      <Signup />
    </ProductProvider>
  );

  const userNameInput = screen.getByLabelText(/user name/i);
  const emailInput = screen.getByLabelText(/email/i);
  const passwordInput = screen.getByLabelText(/password/i);

  fireEvent.change(userNameInput, { target: { value: "rajesh" } });
  fireEvent.change(emailInput, { target: { value: "rajesh@test.com" } });
  fireEvent.change(passwordInput, { target: { value: "123456" } });

  fireEvent.click(screen.getByRole("button", { name: /signup/i }));

  expect(api.post).toHaveBeenCalledTimes(1);
  expect(api.post).toHaveBeenCalledWith("/User/Register", {
    userName: "rajesh",
    email: "rajesh@test.com",
    password: "123456",
  });
});
