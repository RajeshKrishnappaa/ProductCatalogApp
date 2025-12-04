import './Pagination.css';
const Pagination = ({ currentPage, totalItems, pageSize, onPageChange }) => {
  const totalPages = Math.ceil(totalItems / pageSize);

  if (totalPages <= 1) return null;

  return (
    <div className="pagination-container">
      <button
        className="page-btn"
        disabled={currentPage === 1}
        onClick={() => onPageChange(currentPage - 1)}
      >
        Prev
      </button>

      {[...Array(totalPages)].map((_, i) => (
        <button
          key={i + 1}
          className={`page-btn ${currentPage === i + 1 ? "active" : ""}`}
          onClick={() => onPageChange(i + 1)}
        >
          {i + 1}
        </button>
      ))}

      <button
        className="page-btn"
        disabled={currentPage === totalPages}
        onClick={() => onPageChange(currentPage + 1)}
      >
        Next
      </button>
    </div>
  );
};

export default Pagination;
