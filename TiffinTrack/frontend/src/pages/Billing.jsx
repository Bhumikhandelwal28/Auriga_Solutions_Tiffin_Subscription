import { useState } from "react";
import Layout from "../components/Layout";
import api from "../api";

function Billing() {
  const [customerId, setCustomerId] = useState("");
  const [year, setYear] = useState(new Date().getFullYear());
  const [month, setMonth] = useState(new Date().getMonth() + 1);
  const [bill, setBill] = useState(null);

  const calculateBill = async () => {
    if (!customerId) {
      alert("Enter customer ID");
      return;
    }

    try {
      const response = await api.get(
        `/billing/${customerId}?year=${year}&month=${month}`
      );

      setBill(response.data);
    } catch (error) {
      alert(error.response?.data || "Could not calculate bill");
    }
  };

  return (
    <Layout>
      <div className="page-header">
        <div>
          <p className="eyebrow">BILLING</p>
          <h1>Customer Billing</h1>
          <p>Calculate the bill based on days actually served.</p>
        </div>
      </div>

      <div className="dashboard-card billing-form">
        <h3>Calculate Monthly Bill</h3>

        <div className="form-row">
          <div>
            <label>Customer ID</label>
            <input
              type="number"
              placeholder="Enter customer ID"
              value={customerId}
              onChange={(e) => setCustomerId(e.target.value)}
            />
          </div>

          <div>
            <label>Year</label>
            <input
              type="number"
              value={year}
              onChange={(e) => setYear(e.target.value)}
            />
          </div>

          <div>
            <label>Month</label>
            <input
              type="number"
              min="1"
              max="12"
              value={month}
              onChange={(e) => setMonth(e.target.value)}
            />
          </div>

          <button onClick={calculateBill}>
            Calculate Bill
          </button>
        </div>
      </div>

      {bill && (
        <div className="dashboard-card bill-result">
          <p className="eyebrow">MONTHLY BILL</p>

          <h2>{bill.customerName}</h2>

          <p>{bill.planName}</p>

          <div className="bill-grid">
            <div>
              <span>Plan Price</span>
              <strong>₹{bill.monthlyPrice}</strong>
            </div>

            <div>
              <span>Eligible Weekdays</span>
              <strong>{bill.eligibleWeekdays}</strong>
            </div>

            <div>
              <span>Days Served</span>
              <strong>{bill.deliveredDays}</strong>
            </div>

            <div className="final-bill">
              <span>Final Bill</span>
              <strong>₹{bill.totalBill}</strong>
            </div>
          </div>
        </div>
      )}
    </Layout>
  );
}

export default Billing;