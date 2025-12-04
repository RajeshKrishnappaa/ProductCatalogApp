import { useState, useContext, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import ProductContext from "../context/ProductContext";
import api from "../api/api";
import "./AddProduct.css";

const AddProduct = () => {
  const {
    categories,
    loadCategories = () => {},  // fallback for tests
    loadProducts,
    user
  } = useContext(ProductContext);

  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [desc, setDesc] = useState("");
  const [price, setPrice] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [imageFile, setImageFile] = useState(null);

  useEffect(() => {
    loadCategories();
  }, [loadCategories]);

  if (!user || user.role !== "Admin") {
    return <h3 style={{ padding: "2rem" }}>Unauthorized</h3>;
  }

  const handleAdd = async (e) => {
    e.preventDefault();

    if (!imageFile) {
      alert("Please choose an image");
      return;
    }

    const formData = new FormData();
    formData.append("ProductName", name);
    formData.append("Description", desc);
    formData.append("Price", Number(price));
    formData.append("CategoryId", Number(categoryId));
    formData.append("ImageFile", imageFile);

    try {
      await api.post("/Product/UploadImage", formData);

      alert("Product Added Successfully!");

      await loadProducts();
      navigate("/admin");
    } catch (err) {
      console.error("Add product failed:", err);
      alert("Add product failed. Check console for details.");
    }
  };

  return (
    <>
      <div className="back-btn-container">
        <button onClick={() => navigate(-1)} className="back-btn">
          ← Back
        </button>
      </div>

      <main className="add-wrapper">
        <div className="add-card">
          <h2>Add Product</h2>

          <form className="add-form" onSubmit={handleAdd}>
            <label htmlFor="productName">Product Name</label>
            <input
              id="productName"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
            />

            <label htmlFor="description">Description</label>
            <textarea
              id="description"
              required
              value={desc}
              onChange={(e) => setDesc(e.target.value)}
            />

            <label htmlFor="price">Price</label>
            <input
              id="price"
              type="number"
              required
              value={price}
              onChange={(e) => setPrice(e.target.value)}
            />

            <label htmlFor="category">Category</label>
            <select
              id="category"
              required
              value={categoryId}
              onChange={(e) => setCategoryId(e.target.value)}
            >
              <option value="">Select Category</option>
              {categories.map((cat) => (
                <option key={cat.categoryId} value={cat.categoryId}>
                  {cat.categoryName}
                </option>
              ))}
            </select>

            <label htmlFor="image">Upload Image</label>
            <input
              id="image"
              type="file"
              accept="image/*"
              required
              onChange={(e) => setImageFile(e.target.files[0])}
            />

            <button className="btn-add-product" type="submit">
              Add Product
            </button>
          </form>
        </div>
      </main>
    </>
  );
};

export default AddProduct;
