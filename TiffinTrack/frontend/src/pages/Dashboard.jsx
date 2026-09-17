import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Layout from "../components/Layout";
import api from "../api";

function Dashboard() {

  const [customers, setCustomers] = useState(0);
  const [plans, setPlans] = useState(0);
  const [customerList, setCustomerList] = useState([]);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {

    try {

      const customerResponse =
        await api.get("/customers");

      const planResponse =
        await api.get("/plans");

      setCustomers(customerResponse.data.total);
      setPlans(planResponse.data.length);

      setCustomerList(
        customerResponse.data.items || []
      );

    } catch (error) {
      console.error(error);
    }
  };

  const activeCustomers =
    customerList.filter(
      c => c.activeSubscription
    ).length;

  const pausedCustomers =
    customerList.filter(
      c => c.pausedSubscription
    ).length;

  return (

    <Layout>

      <div className="page-header">

        <div>
          <p className="eyebrow">OVERVIEW</p>

          <h1>Good morning, Owner 👋</h1>

          <p>
            Here's what's happening with your tiffin service today.
          </p>
        </div>

        <div className="date-box">
          <strong>
            {new Date().toLocaleDateString(
              "en-IN",
              {
                day: "2-digit",
                month: "short",
                year: "numeric"
              }
            )}
          </strong>
        </div>

      </div>


      <div className="stat-grid">

        <div className="dashboard-card stat">

          <div className="stat-icon">👥</div>

          <div>
            <span>Total Customers</span>
            <h2>{customers}</h2>
          </div>

        </div>


        <div className="dashboard-card stat">

          <div className="stat-icon active-icon">✓</div>

          <div>
            <span>Active Today</span>
            <h2>{activeCustomers}</h2>
          </div>

        </div>


        <div className="dashboard-card stat">

          <div className="stat-icon paused-icon">Ⅱ</div>

          <div>
            <span>Paused</span>
            <h2>{pausedCustomers}</h2>
          </div>

        </div>


        <div className="dashboard-card stat">

          <div className="stat-icon plan-icon">₹</div>

          <div>
            <span>Plans</span>
            <h2>{plans}</h2>
          </div>

        </div>

      </div>


      <div className="dashboard-grid">

        <div className="dashboard-card">

          <div className="card-header">

            <div>
              <h3>Today's Deliveries</h3>
              <p>Customers due for lunch today</p>
            </div>

            <Link to="/deliveries">
              View all →
            </Link>

          </div>

          <div className="delivery-empty">

            <div className="empty-icon">🍱</div>

            <h3>Delivery list</h3>

            <p>
              Run the daily clock to generate today's
              delivery notifications.
            </p>

            <button
              onClick={async () => {
                try {
                  await api.post("/clock");
                  alert("Today's deliveries processed!");
                } catch {
                  alert("Could not process deliveries.");
                }
              }}
            >
              Run Today's Delivery
            </button>

          </div>

        </div>


        <div className="dashboard-card">

          <div className="card-header">

            <div>
              <h3>Quick Actions</h3>
              <p>Common owner tasks</p>
            </div>

          </div>

          <div className="quick-grid">

            <Link to="/customers">
              <strong>+ Customer</strong>
              <span>Add or view customers</span>
            </Link>

            <Link to="/plans">
              <strong>+ Plan</strong>
              <span>Manage tiffin plans</span>
            </Link>

            <Link to="/billing">
              <strong>₹ Billing</strong>
              <span>Calculate customer bills</span>
            </Link>

            <Link to="/import">
              <strong>↑ Import</strong>
              <span>Import customer list</span>
            </Link>

          </div>

        </div>

      </div>

    </Layout>

  );
}

export default Dashboard;