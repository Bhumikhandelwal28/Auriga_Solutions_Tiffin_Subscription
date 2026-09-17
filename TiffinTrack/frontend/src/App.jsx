import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import Customers from "./pages/Customers";
import Plans from "./pages/Plans";
import Billing from "./pages/Billing";
import Deliveries from "./pages/Deliveries";
import ImportCustomers from "./pages/ImportCustomers";

import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <Routes>

        <Route path="/" element={<Navigate to="/dashboard" />} />

        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/customers" element={<Customers />} />
        <Route path="/plans" element={<Plans />} />
        <Route path="/billing" element={<Billing />} />
        <Route path="/deliveries" element={<Deliveries />} />
        <Route path="/import" element={<ImportCustomers />} />

      </Routes>
    </BrowserRouter>
  );
}

export default App;