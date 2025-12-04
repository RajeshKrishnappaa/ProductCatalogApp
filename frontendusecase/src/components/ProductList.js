
import { useContext } from "react";
import ProductContext from "../context/ProductContext";
import ProductCard from "./ProductCard";
import Pagination from "./Pagination";
import './ProductList.css';

const ProductList = () => {
  const { products, totalProducts, currentPage, pageSize, setCurrentPage } = useContext(ProductContext);

  return (
    <section>
      {products.length === 0 && <p>No products found.</p>}

      <div className="product-grid">
        {products.map((p) => (
        <ProductCard key={p.productId} product={p} />
        ))}
    </div>

      <Pagination
      currentPage={currentPage}
      totalItems={totalProducts}
      pageSize={pageSize}
      onPageChange={setCurrentPage}
      />
    </section>
  );
};

export default ProductList;
