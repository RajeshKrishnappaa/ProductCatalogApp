import { useContext, useState, useEffect } from "react";
import ProductContext from "../context/ProductContext";
import ProductCard from "../components/ProductCard";
import { useNavigate } from "react-router-dom";
import api from "../api/api";
import Nav from "../components/Nav";  
import "./AdminDashboard.css";
import Pagination from "../components/Pagination";

const AdminDashboard = () => {
  const { products, deleteProduct, categories, loadCategories,currentPage,
  setCurrentPage,
  totalProducts,
  pageSize} = useContext(ProductContext);
  const navigate = useNavigate();

  const [newCategory, setNewCategory] = useState("");
  const [editMode, setEditMode] = useState(false);
  const [editCatId, setEditCatId] = useState(null);

  useEffect(() => {
    loadCategories();
  }, []);

  const handleCategorySubmit = async (e) => {
    e.preventDefault();
    try{
      if (editMode) {
      await api.put(`/Category/Update/${editCatId}`, {
        categoryId: editCatId,
        categoryName: newCategory,
      });
      alert("Category Updated");
    } else {
      await api.post("/Category/Create", {
        categoryName: newCategory,
      });
      alert("Category Added");
    }
    
    setNewCategory("");
    setEditMode(false);
    loadCategories();
  }catch(err){
    if (err.response && err.response.status === 400) {
      alert(err.response.data || "Category already exists!");
    } else {
      alert("Something went wrong!");
    }
  }
  };

  const deleteCategory = async (id) => {
    if (!window.confirm("Delete this category?")) return;

    await api.delete(`/Category/Delete?id=${id}`);
    loadCategories();
  };

  return (
    <>
    <Nav showfilters={true} />
      <main className="admin-wrapper">
       
        <section className="admin-section">
          <div className="admin-header">
            <h2>Product Management</h2>
            <button onClick={() => navigate("/add")} className="btn-add">
              + Add Product
            </button>
          </div>

          <div className="product-grid">
            {products.map((p) => (
              <div key={p.productId} className="admin-product-card">
                <div className="product-content-wrapper">
                <ProductCard product={p} />
                </div>

                <div className="admin-product-actions">
                  <button className="btn-edit" onClick={() => navigate(`/edit/${p.productId}`)}>
                    Edit
                  </button>

                  <button className="btn-delete" onClick={() => {
                    if (window.confirm("Delete product?")) {
                      deleteProduct(p.productId);
                    }
                  }}>
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>

          <Pagination
            currentPage={currentPage}
            totalItems={totalProducts}
            pageSize={pageSize}
            onPageChange={setCurrentPage}
          />

        </section>
     
        <section className="admin-section">
          <div className="admin-header">
            <h2>Category Management</h2>
          </div>

          <form className="category-form" onSubmit={handleCategorySubmit}>
            <input
              type="text"
              placeholder="Category name"
              value={newCategory}
              required
              onChange={(e) => setNewCategory(e.target.value)}
            />

            <button type="submit" className="btn-add">
              {editMode ? "Update Category" : "Add Category"}
            </button>

            {editMode && (
              <button
                className="btn-cancel"
                type="button"
                onClick={() => {
                  setEditMode(false);
                  setNewCategory("");
                }}
              >
                Cancel
              </button>
            )}
          </form>

          <table className="category-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Category Name</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>
              {categories.map((c) => (
                <tr key={c.categoryId}>
                  <td>{c.categoryId}</td>
                  <td>{c.categoryName}</td>
                  <td className="category-actions">
                    
                    <button
                      className="btn-edit"
                      onClick={() => {
                        setEditMode(true);
                        setEditCatId(c.categoryId);
                        setNewCategory(c.categoryName);
                      }}
                    >
                      Edit
                    </button>

                    <button
                      className="btn-delete"
                      onClick={() => deleteCategory(c.categoryId)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </section>
      </main>
    </>
  );
};

export default AdminDashboard;
