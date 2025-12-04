import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Login from "./Login";
import ProductContext from "../context/ProductContext";
import { mockNavigate } from "react-router-dom";

describe("Login Component", () => {
  const setup = (value) => {
    return render(
      <ProductContext.Provider value={value}>
        <Login />
      </ProductContext.Provider>
    );
  };

  test("success admin login", async () => {
    const user = userEvent.setup();

    const mockLogin = jest.fn().mockResolvedValue({
      ok: true,
      user: { role: "Admin" },
    });

    setup({ login: mockLogin, authError: "" });

    await user.type(screen.getByLabelText(/user name/i), "admin");
    await user.type(screen.getByLabelText(/password/i), "123");
    await user.click(screen.getByRole("button", { name: /login/i }));

    expect(mockNavigate).toHaveBeenCalledWith("/admin");
  });
});
