import { render, screen } from "@testing-library/react";
import Pagination from "./Pagination";

test("renders page buttons", () => {
  render(
    <Pagination
      currentPage={1}
      totalItems={15}
      pageSize={5}
      onPageChange={() => {}}
    />
  );

  expect(screen.getAllByRole("button")).toHaveLength(5); // Prev + 3 pages + Next
});
