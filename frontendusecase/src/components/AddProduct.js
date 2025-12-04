import { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import ProductContext from "../context/ProductContext";
import api from "../api/api";
import "./AddProduct.css";

const AddProduct = () => {
  const { categories, loadCategories, loadProducts } = useContext(ProductContext);

  const [productName, setProductName] = useState("");
  const [price, setPrice] = useState("");
  const [description, setDescription] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [image, setImage] = useState(null);

  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append("productName", productName);
    formData.append("price", price);
    formData.append("description", description);
    formData.append("categoryId", categoryId);
    if (image) formData.append("imageFile", image);

    await api.post("/Product/Add", formData);

    if (typeof loadProducts === "function") {
      await loadProducts(); // 🔥 fix for failing test
    }

    alert("Product Added Successfully!");
    navigate("/admin");
  };

  return (
    <main className="add-product-container">
      <h2>Add Product</h2>

      <form onSubmit={handleSubmit} data-testid="add-form">
        <label>Name</label>
        <input
          data-testid="name-input"
          value={productName}
          onChange={(e) => setProductName(e.target.value)}
          required
        />

        <label>Price</label>
        <input
          data-testid="price-input"
          value={price}
          onChange={(e) => setPrice(e.target.value)}
          required
        />

        <label>Description</label>
        <textarea
          data-testid="desc-input"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          required
        />

        <label>Category</label>
        <select
          data-testid="category-select"
          value={categoryId}
          onChange={(e) => setCategoryId(e.target.value)}
          required
        >
          <option value="">Select</option>
          {categories.map((c) => (
            <option key={c.categoryId} value={c.categoryId}>
              {c.categoryName}
            </option>
          ))}
        </select>

        <label>Image</label>
        <input
          type="file"
          data-testid="image-input"
          onChange={(e) => setImage(e.target.files[0])}
        />

        <button data-testid="submit-btn" type="submit">
          Add Product
        </button>
      </form>
    </main>
  );
};

export default AddProduct;
