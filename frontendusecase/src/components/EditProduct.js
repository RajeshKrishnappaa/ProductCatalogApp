import { useState, useEffect, useContext } from "react";
import { useNavigate, useParams } from "react-router-dom";
import ProductContext from "../context/ProductContext";
import api from "../api/api";
import "./EditProduct.css";

const EditProduct = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user, categories, loadCategories,loadProducts } = useContext(ProductContext);

  const [name, setName] = useState("");
  const [desc, setDesc] = useState("");
  const [price, setPrice] = useState("");
  const [categoryId, setCategoryId] = useState("");

  const [existingImage, setExistingImage] = useState(null);
  const [newImage, setNewImage] = useState(null);
  const [preview, setPreview] = useState(null);

  useEffect(() => {
    loadCategories();
  }, []);

  useEffect(() => {
    const loadProduct = async () => {
      const res = await api.get(`/Product/${id}`);
      const p = res.data;

      setName(p.productName);
      setDesc(p.description);
      setPrice(String(p.price));
      setCategoryId(String(p.categoryId));

      if (p.imageUrl?.startsWith("/")) {
        setExistingImage(`https://localhost:7189${p.imageUrl}`);
      } else {
        setExistingImage(p.imageUrl);
      }

    };

    loadProduct();
  }, [id]);

  if (!user || user.role !== "Admin") return <h2>Unauthorized</h2>;

  const handleUpdate = async (e) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append("ProductName", name);
    formData.append("Description", desc);
    formData.append("Price",Number(price));
    formData.append("CategoryId", Number(categoryId));

    if (newImage) {
      formData.append("ImageFile", newImage);
    }

    await api.put(`/Product/Update/${id}`, formData); 
    alert("Product Updated Successfully!");
    await loadProducts();
    setTimeout(()=>navigate("/admin"),100);
  };

  return (
    <>
    <div className="back-btn-container">
    <button onClick={() => navigate(-1)} className="back-btn">
      ← Back
    </button>
    </div>
      <main className="edit-wrapper">
        <div className="edit-card">
          <h2>Edit Product</h2>

          <form className="edit-form" onSubmit={handleUpdate}>
            <label>Product Name</label>
            <input value={name} onChange={(e) => setName(e.target.value)} required />

            <label>Description</label>
            <textarea value={desc} onChange={(e) => setDesc(e.target.value)} required />

            <label>Price</label>
            <input
              type="number"
              value={price}
              onChange={(e) => setPrice(e.target.value)}
              required
            />

            <label>Category</label>
            <select
              value={categoryId}
              onChange={(e) => setCategoryId(e.target.value)}
              required
            >
              <option value="">Select Category</option>
              {categories.map((c) => (
                <option key={c.categoryId} value={c.categoryId}>
                  {c.categoryName}
                </option>
              ))}
            </select>

            <label>Current Image</label>
            {existingImage && (
              <img src={existingImage} alt="Old" className="preview-img" />
            )}

            <label>Replace Image</label>
            <input
              type="file"
              accept="image/*"
              onChange={(e) => {
                setNewImage(e.target.files[0]);
                setPreview(URL.createObjectURL(e.target.files[0]));
              }}
            />

            {preview && (
              <img src={preview} alt="New preview" className="preview-img" />
            )}

            <button className="btn-update-product" type="submit">
              Update Product
            </button>
          </form>
        </div>
      </main>
    </>
  );
};

export default EditProduct;
