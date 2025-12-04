import { useContext, useEffect } from "react";
import ProductContext from "../context/ProductContext";
import ProductCard from "../components/ProductCard";
import Nav from "../components/Nav";
import "./UserDashboard.css";
import Pagination from "../components/Pagination";

const UserDashboard = () => {
  const { products, loadProducts , currentPage,
  setCurrentPage,
  totalProducts,
  pageSize} = useContext(ProductContext);

  useEffect(() => {
    loadProducts();
  }, []);

  return (
    <>
    <Nav showfilters={true} />
      <main className="user-wrapper">
        <div className="user-header">
          {/* <h2>Available Products</h2> */}
        </div>

        <div className="product-grid">
          {products.length === 0 ? (
            <h3>No products available.</h3>
          ) : (
            products.map((product) => (
              <div key={product.productId} className="user-product-card">
                <ProductCard product={product} />
              </div>
            ))
          )}
        </div>
        <Pagination
          currentPage={currentPage}
          totalItems={totalProducts}
          pageSize={pageSize}
          onPageChange={setCurrentPage}
        />
      </main>
    </>
  );
};

export default UserDashboard;
