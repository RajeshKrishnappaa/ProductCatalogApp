import { useState, useContext } from "react";
import { Link, useNavigate } from "react-router-dom";
import ProductContext from "../context/ProductContext";
import "./Signup.css";
const Signup = () => {
  const { signup } = useContext(ProductContext);

  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const navigate = useNavigate();

  const handle = async (e) => {
    e.preventDefault();

    const res = await signup(userName, email, password);

    if (res.ok) {
      // keep the alert (tests will mock window.alert)
      alert("Signup successful! Please login.");
      navigate("/login");
    }
  };

  return (
    <main className="auth-container">
      <div className="auth-card">
        <h2>Signup</h2>

        <form onSubmit={handle}>
          <label htmlFor="signup-username">User Name</label>
          <input
            id="signup-username"
            type="text"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            required
          />

          <label htmlFor="signup-email">Email</label>
          <input
            id="signup-email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />

          <label htmlFor="signup-password">Password</label>
          <input
            id="signup-password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          <button className="auth-btn" type="submit">Signup</button>
        </form>

        <p>Already have an account?
          <Link to="/login"> Login</Link>
        </p>
      </div>
    </main>
  );
};

export default Signup;
