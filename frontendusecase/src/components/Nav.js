import { useContext } from "react";
import ProductContext from "../context/ProductContext";
import "./Nav.css";
import { FaSearch } from "react-icons/fa";
import { MdFilterList } from "react-icons/md";

const Nav = ({ showfilters = true }) => {
  const {
    search,
    setSearch,
    categories,
    categoryId,
    setCategoryId,
    priceRange,
    setPriceRange,
  } = useContext(ProductContext);

  return (
    <nav className="nav-wrapper">

      <div className="nav-search">
        <FaSearch size={18} color="#555" />
        <input
          type="text"
          placeholder="Search products..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      {showfilters && (
        <div className="nav-filters">
          <MdFilterList size={22} color="#333" />

          <select
            className="nav-select"
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value)}
          >
            <option value="">All Categories</option>
            {categories.map((c) => (
              <option key={c.categoryId} value={c.categoryId}>
                {c.categoryName}
              </option>
            ))}
          </select>

          <div className="price-filter-box">
          <label className="filter-title">Price Range</label>
            <select
              className="price-dropdown"
              onChange={(e) => {
                const val = e.target.value;

                switch (val) {
                  case "0-500":
                    setPriceRange([0, 500]);
                    break;
                  case "500-1000":
                    setPriceRange([500, 1000]);
                    break;
                  case "1000-2000":
                    setPriceRange([1000, 2000]);
                    break;
                  case "2000-5000":
                    setPriceRange([2000, 5000]);
                    break;
                  case "5000+":
                    setPriceRange([5000, 100000]);
                    break;
                  default:
                    setPriceRange([0, 100000]); 
                }
              }}
            >
              <option value="">All Prices</option>
              <option value="0-500">Below ₹500</option>
              <option value="500-1000">₹500 - ₹1000</option>
              <option value="1000-2000">₹1000 - ₹2000</option>
              <option value="2000-5000">₹2000 - ₹5000</option>
              <option value="5000+">Above ₹5000</option>
            </select>
          </div>

        </div>
      )}
    </nav>
  );
};

export default Nav;
