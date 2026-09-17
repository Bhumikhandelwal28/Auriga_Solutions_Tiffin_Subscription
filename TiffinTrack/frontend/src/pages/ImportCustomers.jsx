import { useState } from "react";
import Layout from "../components/Layout";

function ImportCustomers() {
  const [file, setFile] = useState(null);

  const handleImport = () => {
    if (!file) {
      alert("Please select a file first.");
      return;
    }

    alert(
      "File selected. Import API will be connected next."
    );
  };

  return (
    <Layout>
      <div className="page-header">
        <div>
          <p className="eyebrow">DATA IMPORT</p>
          <h1>Import Customers</h1>
          <p>
            Import messy customer data and clean it into subscriptions.
          </p>
        </div>
      </div>

      <div className="dashboard-card import-card">

        <div className="upload-icon">
          ↑
        </div>

        <h2>Import customer list</h2>

        <p>
          Upload a CSV containing customer names, phone numbers,
          plans and start dates.
        </p>

        <input
          type="file"
          accept=".csv"
          onChange={(e) => setFile(e.target.files[0])}
        />

        {file && (
          <p className="selected-file">
            Selected: <strong>{file.name}</strong>
          </p>
        )}

        <button onClick={handleImport}>
          Import Customers
        </button>

        <div className="import-info">
          <div>
            <strong>Imported</strong>
            <span>Valid customers</span>
          </div>

          <div>
            <strong>Deduped</strong>
            <span>Duplicate phones</span>
          </div>

          <div>
            <strong>Rejected</strong>
            <span>Invalid or blank data</span>
          </div>
        </div>

      </div>
    </Layout>
  );
}

export default ImportCustomers;