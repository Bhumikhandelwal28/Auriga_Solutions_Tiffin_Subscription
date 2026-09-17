import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import api from "../api";

function Login() {

  const [phone, setPhone] = useState("");
  const [password, setPassword] = useState("");

  const navigate = useNavigate();

  const handleSubmit = async (e) => {

    e.preventDefault();

    try {

      const response = await api.post("/auth/login", {
        phone,
        password
      });

      localStorage.setItem(
        "user",
        JSON.stringify(response.data)
      );

      navigate("/dashboard");

    } catch (error) {

      alert(
        error.response?.data ||
        "Invalid phone or password"
      );

    }
  };

  return (

    <div className="auth-page">

      <div className="auth-left">

        <div className="auth-brand">
          <div className="brand-icon">T</div>
          <h2>TiffinTrack</h2>
        </div>

        <div className="auth-message">

          <h1>
            Manage every tiffin,
            <br />
            <span>without the headache.</span>
          </h1>

          <p>
            Track subscriptions, pauses, deliveries
            and monthly bills from one place.
          </p>

        </div>

      </div>


      <div className="auth-right">

        <div className="auth-box">

          <div className="mobile-logo">
            <div className="brand-icon">T</div>
            <h2>TiffinTrack</h2>
          </div>

          <h1>Welcome back</h1>

          <p className="auth-subtitle">
            Sign in to manage your tiffin service.
          </p>

          <form onSubmit={handleSubmit}>

            <label>Phone Number</label>

            <input
              type="text"
              placeholder="Enter phone number"
              value={phone}
              onChange={(e) =>
                setPhone(e.target.value)
              }
              required
            />

            <label>Password</label>

            <input
              type="password"
              placeholder="Enter password"
              value={password}
              onChange={(e) =>
                setPassword(e.target.value)
              }
              required
            />

            <button className="primary-btn">
              Sign In
            </button>

          </form>

          <p className="auth-footer">

            Don't have an account?{" "}

            <Link to="/register">
              Create account
            </Link>

          </p>

        </div>

      </div>

    </div>

  );
}

export default Login;