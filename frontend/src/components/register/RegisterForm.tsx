"use client";

import { useState } from "react";
import axios from "axios";
import { Eye, EyeOff } from "lucide-react";

interface RegisterData {
  fullName: string;
  email: string;
  password: string;
  clubName: string;
  invitationCode: string;
}

export default function RegisterForm() {
  const [formData, setFormData] = useState<RegisterData>({
    fullName: "",
    email: "",
    password: "",
    clubName: "",
    invitationCode: "",
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState("");
  const [success, setSuccess] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
    setServerError("");
  };

  const validate = () => {
    const newErrors: Record<string, string> = {};
    if (!formData.fullName.trim()) {
      newErrors.fullName = "Fullt navn er påkrevd";
    }
    if (!formData.email.trim()) {
      newErrors.email = "E-post er påkrevd";
    } else if (!/^\S+@\S+\.\S+$/.test(formData.email)) {
      newErrors.email = "Ugyldig e-post";
    }
    if (!formData.password.trim()) {
      newErrors.password = "Passord er påkrevd";
    } else if (formData.password.length < 6) {
      newErrors.password = "Passord må være minst 6 tegn";
    }
    return newErrors;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const validation = validate();
    if (Object.keys(validation).length > 0) {
      setErrors(validation);
      return;
    }
    setLoading(true);
    setServerError("");
    try {
      await axios.post("/api/Auth/register", {
        fullName: formData.fullName,
        email: formData.email,
        password: formData.password,
        clubName: formData.clubName || undefined,
        invitationCode: formData.invitationCode || undefined,
      });
      setSuccess(true);
      setFormData({
        fullName: "",
        email: "",
        password: "",
        clubName: "",
        invitationCode: "",
      });
    } catch (err: any) {
      setServerError(
        err.response?.data?.message || "Noe gikk galt. Prøv igjen senere."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-slate-800 to-slate-950 p-4">
      <div className="w-full max-w-md space-y-6 rounded-xl border border-slate-700 bg-slate-900/70 p-6 backdrop-blur-md shadow-xl">
        <h1 className="text-center text-3xl font-bold text-white">Registrer deg</h1>
        {serverError && (
          <p className="rounded-md bg-red-500/20 p-2 text-center text-sm text-red-300">
            {serverError}
          </p>
        )}
        {success && (
          <p className="rounded-md bg-green-600/20 p-2 text-center text-sm text-green-300">
            Sjekk e-posten din for å bekrefte kontoen
          </p>
        )}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="fullName" className="block text-sm font-medium text-white mb-1">
              Fullt navn
            </label>
            <input
              id="fullName"
              name="fullName"
              type="text"
              value={formData.fullName}
              onChange={handleChange}
              placeholder="Ditt fulle navn"
              className="w-full rounded-md border border-slate-700 bg-slate-800 text-white p-2 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            {errors.fullName && <p className="mt-1 text-sm text-red-400">{errors.fullName}</p>}
          </div>
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-white mb-1">
              E-post
            </label>
            <input
              id="email"
              name="email"
              type="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="din@epost.no"
              className="w-full rounded-md border border-slate-700 bg-slate-800 text-white p-2 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            {errors.email && <p className="mt-1 text-sm text-red-400">{errors.email}</p>}
          </div>
          <div>
            <label htmlFor="password" className="block text-sm font-medium text-white mb-1">
              Passord
            </label>
            <div className="relative">
              <input
                id="password"
                name="password"
                type={showPassword ? "text" : "password"}
                value={formData.password}
                onChange={handleChange}
                placeholder="Minst 6 tegn"
                className="w-full rounded-md border border-slate-700 bg-slate-800 text-white p-2 pr-10 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-2 top-1/2 -translate-y-1/2 text-slate-400"
              >
                {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
              </button>
            </div>
            {errors.password && <p className="mt-1 text-sm text-red-400">{errors.password}</p>}
          </div>
          <div>
            <label htmlFor="clubName" className="block text-sm font-medium text-white mb-1">
              Klubbenavn (valgfritt)
            </label>
            <input
              id="clubName"
              name="clubName"
              type="text"
              value={formData.clubName}
              onChange={handleChange}
              className="w-full rounded-md border border-slate-700 bg-slate-800 text-white p-2 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label htmlFor="invitationCode" className="block text-sm font-medium text-white mb-1">
              Invitasjonskode (valgfritt)
            </label>
            <input
              id="invitationCode"
              name="invitationCode"
              type="text"
              value={formData.invitationCode}
              onChange={handleChange}
              className="w-full rounded-md border border-slate-700 bg-slate-800 text-white p-2 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="w-full rounded-md bg-blue-600 hover:bg-blue-700 text-white py-2 font-semibold disabled:opacity-50"
          >
            {loading ? "Registrerer..." : "Registrer"}
          </button>
        </form>
      </div>
    </div>
  );
}
