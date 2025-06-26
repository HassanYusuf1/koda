"use client";

import React, { useState } from 'react';
import { Eye, EyeOff, Mail, Lock, AlertCircle } from 'lucide-react';
import '../../styles/login/LoginModule.css'; // Import the CSS file

const LoginPage = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [errors, setErrors] = useState<{[key: string]: string}>({});
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [serverError, setServerError] = useState('');

  // Email validation
  const isValidEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  // Form validation
  const validateForm = () => {
    const newErrors: {[key: string]: string} = {};

    if (!formData.email.trim()) {
      newErrors.email = 'E-post er påkrevd';
    } else if (!isValidEmail(formData.email)) {
      newErrors.email = 'Ugyldig e-postformat';
    }

    if (!formData.password.trim()) {
      newErrors.password = 'Passord er påkrevd';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Passord må være minst 6 tegn';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle input changes
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    // Clear errors for the field being edited
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }

    // Clear server error when user starts typing
    if (serverError) {
      setServerError('');
    }
  };

  // Handle form submission
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    setIsLoading(true);
    setServerError('');

    try {
      const response = await fetch('https://localhost:7223/api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      const data = await response.json();

      if (response.ok) {
        // Success - redirect or handle login success
        console.log('Login successful:', data);
        // Store token if provided
        if (data.data) {
          localStorage.setItem('authToken', data.data);
        }
        // Redirect to dashboard
        window.location.href = '/player';
      } else {
        // Handle different error scenarios
        if (response.status === 401) {
          setServerError('Ugyldig e-post eller passord');
        } else if (response.status === 429) {
          setServerError('For mange forsøk. Prøv igjen senere.');
        } else if (response.status >= 500) {
          setServerError('Serverfeil. Prøv igjen senere.');
        } else {
          setServerError(data.message || 'Noe gikk galt. Prøv igjen.');
        }
      }
    } catch (error) {
      console.error('Login error:', error);
      if (error instanceof Error && error.name === 'AbortError') {
        setServerError('Forespørselen tok for lang tid. Prøv igjen.');
      } else {
        setServerError('Nettverksfeil. Sjekk internettforbindelsen din.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-container">
      {/* Single Centered Login Panel */}
      <div className="login-form-section">
        <div className="login-form-wrapper">
          {/* Header */}
          <div className="login-header">
            <div className="login-logo">
              <svg width="40" height="40" viewBox="0 0 24 24" fill="white">
                <path d="M12 2C13.1 2 14 2.9 14 4C14 5.1 13.1 6 12 6C10.9 6 10 5.1 10 4C10 2.9 10.9 2 12 2ZM21 9V7L19 7.5C18.5 6.8 17.9 6.2 17.2 5.7L17.7 3.7L15.7 2.3L14.7 4.2C14 4.1 13 4.1 12.3 4.2L11.3 2.3L9.3 3.7L9.8 5.7C9.1 6.2 8.5 6.8 8 7.5L6 7V9L8 8.5C8.2 9.2 8.5 9.8 8.9 10.4L7.8 12.3L9.8 13.7L11 11.9C11.6 12 12.4 12 13 11.9L14.2 13.7L16.2 12.3L15.1 10.4C15.5 9.8 15.8 9.2 16 8.5L18 9M12 22C17.5 22 22 17.5 22 12H20C20 16.4 16.4 20 12 20S4 16.4 4 12 7.6 4 12 4V2C6.5 2 2 6.5 2 12S6.5 22 12 22Z"/>
              </svg>
            </div>
            <h1 className="login-title">Logg Inn</h1>
            <p className="login-subtitle">Velkommen tilbake til treningsplattformen</p>
          </div>

          {/* Server Error */}
          {serverError && (
            <div className="server-error">
              <AlertCircle className="text-red-400 flex-shrink-0" size={20} />
              <span className="server-error-text">{serverError}</span>
            </div>
          )}

          {/* Login Form */}
          <div className="login-form">
            {/* Email Field */}
            <div className="input-group">
              <label htmlFor="email" className="input-label">
                E-post
              </label>
              <div className="input-wrapper">
                <div className="input-icon">
                  <Mail size={20} color="#64748b" />
                </div>
                <input
                  id="email"
                  name="email"
                  type="email"
                  autoComplete="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  className={`input-field ${errors.email ? 'error' : ''}`}
                  placeholder="din@epost.no"
                />
              </div>
              {errors.email && (
                <p className="field-error">
                  <AlertCircle size={16} />
                  {errors.email}
                </p>
              )}
            </div>

            {/* Password Field */}
            <div className="input-group">
              <label htmlFor="password" className="input-label">
                Passord
              </label>
              <div className="input-wrapper password-wrapper">
                <div className="input-icon">
                  <Lock size={20} color="#64748b" />
                </div>
                <input
                  id="password"
                  name="password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="current-password"
                  value={formData.password}
                  onChange={handleInputChange}
                  className={`input-field password-field ${errors.password ? 'error' : ''}`}
                  placeholder="Skriv inn passordet ditt"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="password-toggle"
                >
                  {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
              {errors.password && (
                <p className="field-error">
                  <AlertCircle size={16} />
                  {errors.password}
                </p>
              )}
            </div>

            {/* Submit Button */}
            <button
              type="button"
              onClick={handleSubmit}
              disabled={isLoading}
              className="submit-button"
            >
              {isLoading ? (
                <>
                  <div className="loading-spinner"></div>
                  Logger inn...
                </>
              ) : (
                'Logg Inn'
              )}
            </button>

            {/* Forgot Password Link */}
            <div className="forgot-password-wrapper">
              <a 
                href="/forgot-password" 
                className="forgot-password-link"
              >
                Glemt passord?
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;