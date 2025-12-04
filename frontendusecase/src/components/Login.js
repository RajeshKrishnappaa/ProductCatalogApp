import { useState, useContext } from "react";
import { useNavigate, Link } from "react-router-dom";
import ProductContext from "../context/ProductContext";
import './Login.css';

const Login = () => {
  const { login, authError } = useContext(ProductContext);
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");

  const navigate = useNavigate();

  const handle = async (e) => {
    e.preventDefault();

    const res = await login(userName, password);

    if (res.ok) {
      if (res.user.role === "Admin") navigate("/admin");
      else navigate("/user");
    }
  };

  return (
    
    <main className="auth-container">
      <div className="auth-card">
        <h2>Login</h2>

        <form onSubmit={handle}>
          <label htmlFor="username">User Name</label>
          <input id="username" name="username" type="text" value={userName}
            onChange={(e) => setUserName(e.target.value)} required />

          <label htmlFor="password">Password</label>
          <input id="password" name="password" type="password" value={password}
            onChange={(e) => setPassword(e.target.value)} required />

          <button className="auth-btn" type="submit">Login</button>
        </form>

        <p>Don't have an account?  
          <Link to="/signup"> Signup</Link>
        </p>

        {authError && <p className="error">{authError}</p>}
      </div>
    </main>
  );
};

export default Login;
