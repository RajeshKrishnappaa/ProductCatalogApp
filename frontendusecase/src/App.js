import Header from './components/Header';
import Footer from './components/Footer';
import Nav from './components/Nav';
import Login from './components/Login';
import Signup from './components/Signup';
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';
import AdminDashboard from './pages/AdminDashboard';
import UserDashboard from './pages/UserDashboard';
import Missing from './pages/Missing';
import AddProduct from './components/AddProduct';
import EditProduct from './components/EditProduct';
import ProductPage from './components/ProductPage';
import {Routes,Route} from "react-router-dom";
import ProductContext from "./context/ProductContext";
import { useContext } from 'react';

function App() {
  const {user} = useContext(ProductContext);
  return (
    <div className="App">
      <Header title="Product Catalog System"/>
        <Routes>
          <Route path="/" element={<UserDashboard />}/>
          <Route path="/login" element={<Login />} />
          <Route path="/signup" element={<Signup />} />
          <Route element={<ProtectedRoute />}>
            <Route path="/user" element={<UserDashboard />} />
            <Route path="/product/:id" element={<ProductPage />} />
          </Route>
          <Route element={<AdminRoute />}>
            <Route path="/admin" element={<AdminDashboard />} />
            <Route path="/add" element={<AddProduct />} />
            <Route path="/edit/:id" element={<EditProduct />} />
          </Route>
          <Route path="*" element={<Missing />} />
         </Routes>
      <Footer />
      </div>
  );
}

export default App;
