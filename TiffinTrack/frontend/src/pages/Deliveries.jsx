import { useEffect, useState } from "react";
import Layout from "../components/Layout";
import api from "../api";

function Deliveries() {
  const [notifications, setNotifications] = useState([]);

  useEffect(() => {
    loadDeliveries();
  }, []);

  const loadDeliveries = async () => {
    try {
      const response = await api.get("/outbox");
      setNotifications(response.data);
    } catch (error) {
      console.error(error);
    }
  };

  const runClock = async () => {
    try {
      await api.post("/clock");
      await loadDeliveries();
      alert("Today's delivery notifications processed.");
    } catch (error) {
      alert(error.response?.data || "Could not process deliveries.");
    }
  };

  return (
    <Layout>
      <div className="page-header">
        <div>
          <p className="eyebrow">DELIVERIES</p>
          <h1>Today's Deliveries</h1>
          <p>Customers who are due for lunch today.</p>
        </div>

        <button onClick={runClock}>
          Run Today's Clock
        </button>
      </div>

      <div className="dashboard-card">

        <div className="card-header">
          <div>
            <h3>Notification Outbox</h3>
            <p>Customers notified for today's delivery</p>
          </div>

          <strong>{notifications.length}</strong>
        </div>

        {notifications.length === 0 ? (
          <div className="delivery-empty">
            <div className="empty-icon">🍱</div>

            <h3>No deliveries processed yet</h3>

            <p>
              Click "Run Today's Clock" to process today's
              active weekday deliveries.
            </p>
          </div>
        ) : (
          <div className="delivery-list">

            {notifications.map((item) => (
              <div className="delivery-row" key={item.id}>

                <div className="delivery-avatar">
                  🍱
                </div>

                <div>
                  <strong>{item.phone}</strong>
                  <p>{item.message}</p>
                </div>

                <span className="status-active">
                  Sent
                </span>

              </div>
            ))}

          </div>
        )}

      </div>
    </Layout>
  );
}

export default Deliveries;