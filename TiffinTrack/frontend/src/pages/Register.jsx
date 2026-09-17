import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import api from "../api";

function Register() {

  const [name, setName] = useState("");
  const [phone, setPhone] = useState("");
  const [password, setPassword] = useState("");

  const navigate = useNavigate();

  const handleSubmit = async (e) => {

    e.preventDefault();

    try {

      await api.post("/auth/register", {
        name,
        phone,
        password
      });

      alert("Account created successfully!");

      navigate("/login");

    } catch (error) {

      alert(
        error.response?.data ||
        "Registration failed"
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
            Your tiffin business,
            <br />
            <span>all in one place.</span>
          </h1>

          <p>
            Keep customers, subscriptions,
            pauses and billing organized.
          </p>

        </div>

      </div>


      <div className="auth-right">

        <div className="auth-box">

          <h1>Create your account</h1>

          <p className="auth-subtitle">
            Set up your owner account to get started.
          </p>

          <form onSubmit={handleSubmit}>

            <label>Full Name</label>

            <input
              type="text"
              placeholder="Enter your name"
              value={name}
              onChange={(e) =>
                setName(e.target.value)
              }
              required
            />

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
              placeholder="Create password"
              value={password}
              onChange={(e) =>
                setPassword(e.target.value)
              }
              required
            />

            <button className="primary-btn">
              Create Account
            </button>

          </form>

          <p className="auth-footer">

            Already have an account?{" "}

            <Link to="/login">
              Sign in
            </Link>

          </p>

        </div>

      </div>

    </div>

  );
}

export default Register;