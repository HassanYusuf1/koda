"use client";

import React, { useState } from 'react';
import { Mail, AlertCircle, ArrowLeft, CheckCircle, Send } from 'lucide-react';
import { useRouter } from 'next/navigation';
import '../../styles/login/LoginModule.css';

const ForgotPasswordPage = () => {
  const router = useRouter();
  const [email, setEmail] = useState('');
  const [errors, setErrors] = useState<{[key: string]: string}>({});
  const [isLoading, setIsLoading] = useState(false);
  const [serverError, setServerError] = useState('');
  const [isEmailSent, setIsEmailSent] = useState(false);

  // Email validation
  const isValidEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  // Form validation
  const validateForm = () => {
    const newErrors: {[key: string]: string} = {};

    if (!email.trim()) {
      newErrors.email = 'E-post er påkrevd';
    } else if (!isValidEmail(email)) {
      newErrors.email = 'Ugyldig e-postformat';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle input changes
  const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setEmail(value);

    // Clear errors when user starts typing
    if (errors.email) {
      setErrors(prev => ({
        ...prev,
        email: ''
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
      const response = await fetch('https://localhost:7223/api/auth/forgot-password', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email }),
      });

      const data = await response.json();

      if (response.ok) {
        // Success - show confirmation
        console.log('Password reset email sent:', data);
        setIsEmailSent(true);
      } else {
        // Handle different error scenarios
        if (response.status === 404) {
          setServerError('Ingen bruker funnet med denne e-postadressen');
        } else if (response.status === 429) {
          setServerError('For mange forsøk. Prøv igjen senere.');
        } else if (response.status >= 500) {
          setServerError('Serverfeil. Prøv igjen senere.');
        } else {
          setServerError(data.message || 'Noe gikk galt. Prøv igjen.');
        }
      }
    } catch (error) {
      console.error('Forgot password error:', error);
      if (error instanceof Error && error.name === 'AbortError') {
        setServerError('Forespørselen tok for lang tid. Prøv igjen.');
      } else {
        setServerError('Nettverksfeil. Sjekk internettforbindelsen din.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  // Navigate back to login
  const handleBackToLogin = () => {
    router.push('/login');
  };

  // If email was sent successfully, show confirmation screen
  if (isEmailSent) {
    return (
      <div className="login-container">
        <div className="login-form-section">
          <div className="login-form-wrapper">
            {/* Header */}
            <div className="login-header">
              <div className="login-logo success-logo">
                <CheckCircle size={40} color="white" />
              </div>
              <h1 className="login-title">E-post sendt!</h1>
              <p className="login-subtitle">
                Vi har sendt instruksjoner for å tilbakestille passordet til <strong>{email}</strong>
              </p>
            </div>

            {/* Instructions */}
            <div className="success-content">
              <div className="success-instructions">
                <h3>Hva skjer nå?</h3>
                <ul>
                  <li>Sjekk innboksen din (og spam-mappen)</li>
                  <li>Klikk på lenken i e-posten</li>
                  <li>Følg instruksjonene for å lage et nytt passord</li>
                  <li>Logg inn med ditt nye passord</li>
                </ul>
              </div>

              <p className="resend-info">
                Fikk du ikke e-posten? Sjekk spam-mappen eller 
                <button
                  type="button"
                  onClick={() => setIsEmailSent(false)}
                  className="resend-link"
                >
                  prøv igjen
                </button>
              </p>

              <button
                type="button"
                onClick={handleBackToLogin}
                className="submit-button"
              >
                <ArrowLeft size={20} />
                Tilbake til innlogging
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="login-container">
      <div className="login-form-section">
        <div className="login-form-wrapper">
          {/* Header */}
          <div className="login-header">
            <button 
              onClick={handleBackToLogin}
              className="back-button"
            >
              <ArrowLeft size={20} />
              Tilbake til innlogging
            </button>
            <div className="login-logo">
              <svg width="40" height="40" viewBox="0 0 24 24" fill="white">
                <path d="M12 2C13.1 2 14 2.9 14 4C14 5.1 13.1 6 12 6C10.9 6 10 5.1 10 4C10 2.9 10.9 2 12 2ZM21 9V7L19 7.5C18.5 6.8 17.9 6.2 17.2 5.7L17.7 3.7L15.7 2.3L14.7 4.2C14 4.1 13 4.1 12.3 4.2L11.3 2.3L9.3 3.7L9.8 5.7C9.1 6.2 8.5 6.8 8 7.5L6 7V9L8 8.5C8.2 9.2 8.5 9.8 8.9 10.4L7.8 12.3L9.8 13.7L11 11.9C11.6 12 12.4 12 13 11.9L14.2 13.7L16.2 12.3L15.1 10.4C15.5 9.8 15.8 9.2 16 8.5L18 9M12 22C17.5 22 22 17.5 22 12H20C20 16.4 16.4 20 12 20S4 16.4 4 12 7.6 4 12 4V2C6.5 2 2 6.5 2 12S6.5 22 12 22Z"/>
              </svg>
            </div>
            <h1 className="login-title">Glemt passord?</h1>
            <p className="login-subtitle">
              Ingen problem! Skriv inn e-postadressen din så sender vi deg instruksjoner for å tilbakestille passordet.
            </p>
          </div>

          {/* Server Error */}
          {serverError && (
            <div className="server-error">
              <AlertCircle className="text-red-400 flex-shrink-0" size={20} />
              <span className="server-error-text">{serverError}</span>
            </div>
          )}

          {/* Forgot Password Form */}
          <form onSubmit={handleSubmit} className="login-form">
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
                  value={email}
                  onChange={handleEmailChange}
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

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isLoading}
              className="submit-button"
            >
              {isLoading ? (
                <>
                  <div className="loading-spinner"></div>
                  Sender e-post...
                </>
              ) : (
                <>
                  <Send size={20} />
                  Send tilbakestillingslenke
                </>
              )}
            </button>

            {/* Back to Login Link */}
            <div className="forgot-password-wrapper">
              <span className="text-gray-400 text-sm">Husket du passordet? </span>
              <button
                type="button"
                onClick={handleBackToLogin}
                className="forgot-password-link"
              >
                Logg inn her
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ForgotPasswordPage;