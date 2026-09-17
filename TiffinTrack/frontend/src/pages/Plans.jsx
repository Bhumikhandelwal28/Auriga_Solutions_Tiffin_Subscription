import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../api";

function Plans() {
  const [plans, setPlans] = useState([]);

  useEffect(() => {
    loadPlans();
  }, []);

  const loadPlans = async () => {
    try {
      const response = await api.get("/plans");
      setPlans(response.data);
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
        <h1>Tiffin Plans</h1>

        <div className="cards">
          {plans.map((plan) => (
            <div className="plan-card" key={plan.id}>
              <h2>{plan.name}</h2>

              <p>{plan.description}</p>

              <h3>₹{plan.monthlyPrice}/month</h3>

              <span>
                {plan.isActive ? "Active" : "Inactive"}
              </span>
            </div>
          ))}
        </div>
      </main>
    </div>
  );
}

export default Plans;