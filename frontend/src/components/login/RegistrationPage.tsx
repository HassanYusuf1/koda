"use client";

import React, { useState } from 'react';
import { Eye, EyeOff, Mail, Lock, AlertCircle, User, ArrowLeft, Calendar, MapPin, Users } from 'lucide-react';
import { useRouter } from 'next/navigation';
import '../../styles/login/LoginModule.css';

const RegisterPage = () => {
  const router = useRouter();
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    password: '',
    confirmPassword: '',
    position: '',
    dateOfBirth: '',
    role: 'Player'
  });
  const [errors, setErrors] = useState<{[key: string]: string}>({});
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [serverError, setServerError] = useState('');

  // Email validation
  const isValidEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  // Password validation
  const isValidPassword = (password: string) => {
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/;
    return password.length >= 8 && passwordRegex.test(password);
  };

  // Form validation
  const validateForm = () => {
    const newErrors: {[key: string]: string} = {};

    // Full name validation
    if (!formData.fullName.trim()) {
      newErrors.fullName = 'Fullt navn er påkrevd';
    } else if (formData.fullName.trim().length < 2) {
      newErrors.fullName = 'Fullt navn må være minst 2 tegn';
    } else if (formData.fullName.length > 100) {
      newErrors.fullName = 'Fullt navn kan ikke være mer enn 100 tegn';
    }

    // Email validation
    if (!formData.email.trim()) {
      newErrors.email = 'E-post er påkrevd';
    } else if (!isValidEmail(formData.email)) {
      newErrors.email = 'Ugyldig e-postformat';
    }

    // Password validation
    if (!formData.password.trim()) {
      newErrors.password = 'Passord er påkrevd';
    } else if (!isValidPassword(formData.password)) {
      newErrors.password = 'Passord må være minst 8 tegn og inneholde store og små bokstaver, tall og spesialtegn';
    }

    // Confirm password validation
    if (!formData.confirmPassword.trim()) {
      newErrors.confirmPassword = 'Bekreft passord er påkrevd';
    } else if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passordene stemmer ikke overens';
    }

    // Position validation (optional but if provided, check length)
    if (formData.position && formData.position.length > 50) {
      newErrors.position = 'Posisjon kan ikke være mer enn 50 tegn';
    }

    // Role validation
    if (!['Player', 'Coach', 'Admin'].includes(formData.role)) {
      newErrors.role = 'Ugyldig rolle valgt';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle input changes
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
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
      // Prepare data according to your RegisterDto
      const registerData = {
        email: formData.email,
        password: formData.password,
        fullName: formData.fullName,
        position: formData.position || null,
        dateOfBirth: formData.dateOfBirth ? new Date(formData.dateOfBirth).toISOString() : null,
        role: formData.role
      };

      const response = await fetch('https://localhost:7223/api/auth/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(registerData),
      });

      const data = await response.json();

      if (response.ok) {
        // Success - show success message and redirect to login
        console.log('Registration successful:', data);
        alert('Registrering vellykket! Du kan nå logge inn.');
        router.push('/login');
      } else {
        // Handle different error scenarios
        if (response.status === 409) {
          setServerError('En bruker med denne e-postadressen eksisterer allerede');
        } else if (response.status === 400) {
          // Handle validation errors from server
          if (data.data && Array.isArray(data.data)) {
            setServerError(data.data.join(', '));
          } else {
            setServerError(data.message || 'Ugyldig data oppgitt');
          }
        } else if (response.status >= 500) {
          setServerError('Serverfeil. Prøv igjen senere.');
        } else {
          setServerError(data.message || 'Noe gikk galt. Prøv igjen.');
        }
      }
    } catch (error) {
      console.error('Registration error:', error);
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

  return (
    <div className="login-container">
      <div className="login-form-section">
        <div className="login-form-wrapper register-form">
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
            <h1 className="login-title">Opprett Konto</h1>
            <p className="login-subtitle">Bli med i treningsplattformen</p>
          </div>

          {/* Server Error */}
          {serverError && (
            <div className="server-error">
              <AlertCircle className="text-red-400 flex-shrink-0" size={20} />
              <span className="server-error-text">{serverError}</span>
            </div>
          )}

          {/* Registration Form */}
          <form onSubmit={handleSubmit} className="login-form">
            {/* Full Name Field */}
            <div className="input-group">
              <label htmlFor="fullName" className="input-label">
                Fullt navn *
              </label>
              <div className="input-wrapper">
                <div className="input-icon">
                  <User size={20} color="#64748b" />
                </div>
                <input
                  id="fullName"
                  name="fullName"
                  type="text"
                  autoComplete="name"
                  value={formData.fullName}
                  onChange={handleInputChange}
                  className={`input-field ${errors.fullName ? 'error' : ''}`}
                  placeholder="Skriv inn ditt fulle navn"
                />
              </div>
              {errors.fullName && (
                <p className="field-error">
                  <AlertCircle size={16} />
                  {errors.fullName}
                </p>
              )}
            </div>

            {/* Email Field */}
            <div className="input-group">
              <label htmlFor="email" className="input-label">
                E-post *
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
                Passord *
              </label>
              <div className="input-wrapper password-wrapper">
                <div className="input-icon">
                  <Lock size={20} color="#64748b" />
                </div>
                <input
                  id="password"
                  name="password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="new-password"
                  value={formData.password}
                  onChange={handleInputChange}
                  className={`input-field password-field ${errors.password ? 'error' : ''}`}
                  placeholder="Minimum 8 tegn med store/små bokstaver, tall og tegn"
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

            {/* Confirm Password Field */}
            <div className="input-group">
              <label htmlFor="confirmPassword" className="input-label">
                Bekreft passord *
              </label>
              <div className="input-wrapper password-wrapper">
                <div className="input-icon">
                  <Lock size={20} color="#64748b" />
                </div>
                <input
                  id="confirmPassword"
                  name="confirmPassword"
                  type={showConfirmPassword ? 'text' : 'password'}
                  autoComplete="new-password"
                  value={formData.confirmPassword}
                  onChange={handleInputChange}
                  className={`input-field password-field ${errors.confirmPassword ? 'error' : ''}`}
                  placeholder="Gjenta passordet ditt"
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="password-toggle"
                >
                  {showConfirmPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
              {errors.confirmPassword && (
                <p className="field-error">
                  <AlertCircle size={16} />
                  {errors.confirmPassword}
                </p>
              )}
            </div>

            {/* Position Field */}
            <div className="input-group">
              <label htmlFor="position" className="input-label">
                Posisjon (valgfritt)
              </label>
              <div className="input-wrapper">
                <div className="input-icon">
                  <MapPin size={20} color="#64748b" />
                </div>
                <input
                  id="position"
                  name="position"
                  type="text"
                  value={formData.position}
                  onChange={handleInputChange}
                  className={`input-field ${errors.position ? 'error' : ''}`}
                  placeholder="F.eks. Målvakt, Forsvar, Midtbane, Angrep"
                />
              </div>
              {errors.position && (
                <p className="field-error">
                  <AlertCircle size={16} />
                  {errors.position}
                </p>
              )}
            </div>

            {/* Date of Birth Field */}
            <div className="input-group">
              <label htmlFor="dateOfBirth" className="input-label">
                Fødselsdato (valgfritt)
              </label>
              <div className="input-wrapper">
                <div className="input-icon">
                  <Calendar size={20} color="#64748b" />
                </div>
                <input
                  id="dateOfBirth"
                  name="dateOfBirth"
                  type="date"
                  value={formData.dateOfBirth}
                  onChange={handleInputChange}
                  className="input-field"
                />
              </div>
            </div>

            {/* Role Field */}
            <div className="input-group">
              <label htmlFor="role" className="input-label">
                Rolle *
              </label>
              <div className="input-wrapper">
                <div className="input-icon">
                  <Users size={20} color="#64748b" />
                </div>
                <select
                  id="role"
                  name="role"
                  value={formData.role}
                  onChange={handleInputChange}
                  className={`input-field select-field ${errors.role ? 'error' : ''}`}
                >
                  <option value="Player">Spiller</option>
                  <option value="Coach">Trener</option>
                  <option value="Admin">Administrator</option>
                </select>
              </div>
              {errors.role && (
                <p className="field-error">
                  <AlertCircle size={16} />
                  {errors.role}
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
                  Oppretter konto...
                </>
              ) : (
                'Opprett konto'
              )}
            </button>

            {/* Login Link */}
            <div className="forgot-password-wrapper">
              <span className="text-gray-400 text-sm">Har du allerede en konto? </span>
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

export default RegisterPage;