import { useParams, useNavigate, Link } from "react-router-dom";
import { useContext } from "react";
import ProductContext from "../context/ProductContext";
import Nav from "./Nav";
import './ProductPage.css';

const ProductPage = () => {
  const { id } = useParams();
  const { products, deleteProduct, user } = useContext(ProductContext);
  const navigate = useNavigate();

  const product = products.find((p) => p.productId === Number(id));

  if (!product) return <h2>Product not found</h2>;

  const handleRemove = async () => {
    if (window.confirm("Delete this product?")) {
      await deleteProduct(product.productId);
      navigate(user.role === "Admin" ? "/admin" : "/user");
    }
  };

  return (
    <>
      <Nav />
      <div className="back-btn-container">
        <button onClick={() => navigate(-1)} className="back-btn">
          ← Back
        </button>
      </div>

      <main className="PostPage">
        <article className="post">
          <h2>{product.productName}</h2>

          {product.imageUrl && (
            <img
              src={product.imageUrl}
              alt={product.productName}
              style={{ width: "250px", borderRadius: "10px" }}
            />
          )}

          <p>
            Category: <span>{product.category?.categoryName}</span>
          </p>

          <p>
            Price: ₹<span>{product.price}</span>
          </p>

          <p>
            Description: <span>{product.description}</span>
          </p>

          {user && user.role === "Admin" && (
            <div className="post-buttons">
              <Link to={`/edit/${product.productId}`}>
                <button className="editButton">Edit</button>
              </Link>
              <button className="deleteButton" data-testid="delete-btn" onClick={handleRemove}>
                Delete
              </button>
            </div>
          )}
        </article>
      </main>
    </>
  );
};

export default ProductPage;
