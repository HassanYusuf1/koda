"use client";

import React, { useState } from 'react';
import { Eye, EyeOff, Mail, Lock, AlertCircle } from 'lucide-react';

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
    <div className="min-h-screen flex">
      {/* Left Side - Login Form */}
      <div className="flex-1 flex items-center justify-center p-8 bg-gray-900">
        <div className="w-full max-w-md space-y-8">
          {/* Header */}
          <div className="text-center">
            <h1 className="text-4xl font-bold text-white mb-2">Logg Inn</h1>
            <p className="text-gray-400">Velkommen tilbake til treningsplattformen</p>
          </div>

          {/* Server Error */}
          {serverError && (
            <div className="bg-red-900/20 border border-red-500 rounded-lg p-4 flex items-center gap-3">
              <AlertCircle className="text-red-400 flex-shrink-0" size={20} />
              <span className="text-red-200 text-sm">{serverError}</span>
            </div>
          )}

          {/* Login Form */}
          <div className="space-y-6">
            {/* Email Field */}
            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-300 mb-2">
                E-post
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Mail className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  id="email"
                  name="email"
                  type="email"
                  autoComplete="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  className={`w-full pl-10 pr-4 py-3 bg-gray-800 border rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 transition-colors ${
                    errors.email 
                      ? 'border-red-500 focus:ring-red-500/50' 
                      : 'border-gray-600 focus:ring-blue-500/50 focus:border-blue-500'
                  }`}
                  placeholder="din@epost.no"
                />
              </div>
              {errors.email && (
                <p className="mt-2 text-sm text-red-400 flex items-center gap-1">
                  <AlertCircle size={16} />
                  {errors.email}
                </p>
              )}
            </div>

            {/* Password Field */}
            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-300 mb-2">
                Passord
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Lock className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  id="password"
                  name="password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="current-password"
                  value={formData.password}
                  onChange={handleInputChange}
                  className={`w-full pl-10 pr-12 py-3 bg-gray-800 border rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 transition-colors ${
                    errors.password 
                      ? 'border-red-500 focus:ring-red-500/50' 
                      : 'border-gray-600 focus:ring-blue-500/50 focus:border-blue-500'
                  }`}
                  placeholder="Skriv inn passordet ditt"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-300 transition-colors"
                >
                  {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
              {errors.password && (
                <p className="mt-2 text-sm text-red-400 flex items-center gap-1">
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
              className="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-semibold hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
              {isLoading ? (
                <>
                  <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                  Logger inn...
                </>
              ) : (
                'Logg Inn'
              )}
            </button>

            {/* Forgot Password Link */}
            <div className="text-center">
              <a 
                href="/forgot-password" 
                className="text-blue-400 hover:text-blue-300 text-sm transition-colors"
              >
                Glemt passord?
              </a>
            </div>
          </div>
        </div>
      </div>

      {/* Right Side - Background Image */}
      <div className="hidden lg:flex flex-1 relative">
        <div 
          className="w-full bg-cover bg-center relative"
          style={{
            backgroundImage: `url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 800 600"><defs><pattern id="grid" width="40" height="40" patternUnits="userSpaceOnUse"><path d="M 40 0 L 0 0 0 40" fill="none" stroke="%23064e3b" stroke-width="1"/></pattern></defs><rect width="100%" height="100%" fill="%23065f46"/><rect width="100%" height="100%" fill="url(%23grid)"/><circle cx="400" cy="300" r="80" fill="%23ffffff" opacity="0.1"/><rect x="350" y="250" width="100" height="100" fill="%23ffffff" opacity="0.05" rx="10"/></svg>')`
          }}
        >
          {/* Overlay */}
          <div className="absolute inset-0 bg-gradient-to-br from-blue-900/40 to-green-900/40"></div>
          
          {/* Content Overlay */}
          <div className="absolute inset-0 flex items-center justify-center">
            <div className="text-center text-white p-8">
              <div className="mb-6">
                <div className="w-24 h-24 mx-auto mb-4 bg-white/10 rounded-full flex items-center justify-center backdrop-blur-sm">
                  <svg width="48" height="48" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M12 2C13.1 2 14 2.9 14 4C14 5.1 13.1 6 12 6C10.9 6 10 5.1 10 4C10 2.9 10.9 2 12 2ZM21 9V7L19 7.5C18.5 6.8 17.9 6.2 17.2 5.7L17.7 3.7L15.7 2.3L14.7 4.2C14 4.1 13 4.1 12.3 4.2L11.3 2.3L9.3 3.7L9.8 5.7C9.1 6.2 8.5 6.8 8 7.5L6 7V9L8 8.5C8.2 9.2 8.5 9.8 8.9 10.4L7.8 12.3L9.8 13.7L11 11.9C11.6 12 12.4 12 13 11.9L14.2 13.7L16.2 12.3L15.1 10.4C15.5 9.8 15.8 9.2 16 8.5L18 9M12 22C17.5 22 22 17.5 22 12H20C20 16.4 16.4 20 12 20S4 16.4 4 12 7.6 4 12 4V2C6.5 2 2 6.5 2 12S6.5 22 12 22Z"/>
                  </svg>
                </div>
              </div>
              <h2 className="text-3xl font-bold mb-4">Tren Smart</h2>
              <p className="text-lg opacity-90 max-w-sm mx-auto">
                Digital plattform for fotballtrening og spillerutvikling
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Mobile: Show background at top on small screens */}
      <div className="lg:hidden absolute top-0 left-0 right-0 h-48 -z-10">
        <div 
          className="w-full h-full bg-cover bg-center"
          style={{
            backgroundImage: `url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 800 300"><defs><pattern id="grid-mobile" width="30" height="30" patternUnits="userSpaceOnUse"><path d="M 30 0 L 0 0 0 30" fill="none" stroke="%23064e3b" stroke-width="1"/></pattern></defs><rect width="100%" height="100%" fill="%23065f46"/><rect width="100%" height="100%" fill="url(%23grid-mobile)"/></svg>')`
          }}
        >
          <div className="absolute inset-0 bg-gradient-to-b from-blue-900/60 to-gray-900"></div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;