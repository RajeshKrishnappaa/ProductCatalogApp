const React = require("react");

const mockNavigate = jest.fn();

module.exports = {
  __esModule: true,
  useNavigate: () => mockNavigate,
  useParams: () => ({ id: "1" }),
  BrowserRouter: ({ children }) => <div>{children}</div>,
  MemoryRouter: ({ children }) => <div>{children}</div>,
  Link: ({ children, to }) => <a href={to}>{children}</a>,
  Routes: ({ children }) => <div>{children}</div>,
  Route: ({ element }) => element,
  mockNavigate,
};
