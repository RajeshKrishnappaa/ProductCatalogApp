
import { useContext } from "react";
import { Navigate, Outlet } from "react-router-dom";
import ProductContext from "../context/ProductContext";

const ProtectedRoute = () => {
  const { user } = useContext(ProductContext);
  return user ? <Outlet /> : <Navigate to="/" replace />; 
};

export default ProtectedRoute;
