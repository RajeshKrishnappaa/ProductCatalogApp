const React = require("react");
const mockNavigate = jest.fn();

module.exports = {
  __esModule: true,

  BrowserRouter: ({ children }) => <div>{children}</div>,
  MemoryRouter: ({ children }) => <div>{children}</div>,

  Routes: ({ children }) => <div>{children}</div>,
  Route: ({ element }) => element,

  useNavigate: () => mockNavigate,
  useParams: () => ({ id: "1" }),

  Link: ({ children, to }) => <a href={to}>{children}</a>,

  mockNavigate,
};
