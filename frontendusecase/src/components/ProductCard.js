import { useContext } from "react";
import { useNavigate } from "react-router-dom";
import ProductContext from "../context/ProductContext";
import "./ProductCard.css";

const ProductCard = ({ product }) => {
  const { user } = useContext(ProductContext);
  const navigate = useNavigate();

  const handleClick = () => {
    if (!user) {
      navigate("/login");
      return;
    }

    navigate(`/product/${product.productId}`);
  };

  return (
    <article className="product-card" onClick={handleClick} style={{ cursor: "pointer" }}>

      <div className="product-image-wrapper">
        <img
          src={
            product.imageUrl?.startsWith("/")
              ? `https://localhost:7189${product.imageUrl}`
              : product.imageUrl
          }
          alt={product.productName}
          className="product-image"
        />
      </div>

      <h3 className="product-title">{product.productName}</h3>
      <p className="product-price">₹ {product.price}</p>
      <p className="product-description">
        {(product.description || "").slice(0, 60)}...
      </p>
    </article>
  );
};

export default ProductCard;
