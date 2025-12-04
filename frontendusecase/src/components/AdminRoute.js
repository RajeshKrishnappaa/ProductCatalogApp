
import { useContext } from "react";
import { Navigate, Outlet } from "react-router-dom";
import ProductContext from "../context/ProductContext";

const AdminRoute = () => {
  const { user } = useContext(ProductContext);
  return user && user.role === "Admin"
    ? <Outlet />
    : <Navigate to="/" replace />;
};

export default AdminRoute;
