import { NavLink, useNavigate } from "react-router-dom";

function Layout({ children }) {

  const navigate = useNavigate();

  const logout = () => {
    localStorage.removeItem("user");
    navigate("/login");
  };

  return (
    <div className="app-layout">

      <aside className="sidebar">

        <div className="brand">
          <div className="brand-icon">T</div>
          <div>
            <h2>TiffinTrack</h2>
            <span>Owner Portal</span>
          </div>
        </div>

        <nav>

          <p className="menu-title">MAIN</p>

          <NavLink to="/dashboard">
            <span>⌂</span>
            Overview
          </NavLink>

          <NavLink to="/customers">
            <span>◉</span>
            Customers
          </NavLink>

          <NavLink to="/deliveries">
            <span>▣</span>
            Today's Deliveries
          </NavLink>

          <p className="menu-title">MANAGEMENT</p>

          <NavLink to="/plans">
            <span>◆</span>
            Plans
          </NavLink>

          <NavLink to="/billing">
            <span>₹</span>
            Billing
          </NavLink>

          <NavLink to="/import">
            <span>↑</span>
            Import Customers
          </NavLink>

        </nav>

        <div className="sidebar-bottom">

          <div className="owner-profile">
            <div className="avatar">O</div>

            <div>
              <strong>Owner</strong>
              <span>Tiffin Service</span>
            </div>
          </div>

          <button className="logout-btn" onClick={logout}>
            Logout
          </button>

        </div>

      </aside>

      <main className="main-content">
        {children}
      </main>

    </div>
  );
}

export default Layout;