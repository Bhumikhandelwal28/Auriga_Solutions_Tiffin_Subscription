import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../api";

function Customers() {
  const [customers, setCustomers] = useState([]);

  useEffect(() => {
    loadCustomers();
  }, []);

  const loadCustomers = async () => {
    try {
      const response = await api.get("/customers");

      setCustomers(response.data.items);
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div className="dashboard">
      <nav>
        <h2>TiffinTrack</h2>

        <div>
          <Link to="/dashboard">Dashboard</Link>
          <Link to="/plans">Plans</Link>
          <Link to="/customers">Customers</Link>
        </div>
      </nav>

      <main>
        <h1>Customers</h1>

        <div className="customer-list">
          {customers.map((customer) => (
            <div className="customer-card" key={customer.id}>
              <div>
                <h3>{customer.name}</h3>
                <p>{customer.phone}</p>
              </div>

              <div>
                {customer.activeSubscription ? (
                  <span>
                    {customer.activeSubscription.plan}
                  </span>
                ) : (
                  <span>No active subscription</span>
                )}
              </div>
            </div>
          ))}
        </div>
      </main>
    </div>
  );
}

export default Customers;