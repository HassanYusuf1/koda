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
    } else if (formData.password.length < 8) {
      newErrors.password = "Passord må være minst 8 tegn";
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
      await axios.post("http://localhost:5227/api/Auth/register", {
        email: formData.email,
        password: formData.password,
        fullName: formData.fullName,
        role: "PlatformAdmin",
        position: null,
        team: null,
        dateOfBirth: null,
        clubName: formData.clubName || null,
        clubId: null,
        teamId: null,
        invitationCode: formData.invitationCode || null,
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
      if (axios.isAxiosError(err)) {
        console.error("🪵 Axios error response:", err.response?.data);

        const backendData = err.response?.data;

        if (Array.isArray(backendData?.data)) {
          setServerError(backendData.data.join(" "));
        } else {
          setServerError(backendData?.message || "Noe gikk galt. Prøv igjen senere.");
        }
      } else {
        setServerError("Noe gikk galt. Prøv igjen senere.");
      }
    } finally {
      setLoading(false);
    }
  };

return (
  <div className="flex items-center justify-center min-h-screen bg-gradient-to-tr from-[#e6ecf5] to-[#d4ddef] dark:from-slate-900 dark:to-slate-950 p-4">
    <div className="w-full max-w-md bg-white/80 dark:bg-white/10 backdrop-blur-xl rounded-3xl border border-slate-200 dark:border-white/20 shadow-2xl px-8 py-10 space-y-6">
      <h1 className="text-center text-3xl font-semibold text-slate-900 dark:text-white">Registrer deg</h1>

      {serverError && (
        <p className="rounded-md bg-red-100 dark:bg-red-500/20 text-center text-sm text-red-600 dark:text-red-300 py-2 px-4">
          {serverError}
        </p>
      )}

      {success && (
        <p className="rounded-md bg-green-100 dark:bg-green-500/20 text-center text-sm text-green-700 dark:text-green-300 py-2 px-4">
          Sjekk e-posten din for å bekrefte kontoen
        </p>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="fullName" className="block text-sm font-medium text-slate-700 dark:text-white mb-1">
            Fullt navn
          </label>
          <input
            id="fullName"
            name="fullName"
            type="text"
            value={formData.fullName}
            onChange={handleChange}
            placeholder="Ditt fulle navn"
            className="w-full rounded-lg border border-slate-300 dark:border-slate-700 bg-white/90 dark:bg-slate-800 text-slate-900 dark:text-white p-3 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {errors.fullName && <p className="mt-1 text-sm text-red-500">{errors.fullName}</p>}
        </div>

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-slate-700 dark:text-white mb-1">
            E-post
          </label>
          <input
            id="email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
            placeholder="din@epost.no"
            className="w-full rounded-lg border border-slate-300 dark:border-slate-700 bg-white/90 dark:bg-slate-800 text-slate-900 dark:text-white p-3 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {errors.email && <p className="mt-1 text-sm text-red-500">{errors.email}</p>}
        </div>

        <div>
          <label htmlFor="password" className="block text-sm font-medium text-slate-700 dark:text-white mb-1">
            Passord
          </label>
          <div className="relative">
            <input
              id="password"
              name="password"
              type={showPassword ? "text" : "password"}
              value={formData.password}
              onChange={handleChange}
              placeholder="Minst 8 tegn"
              className="w-full rounded-lg border border-slate-300 dark:border-slate-700 bg-white/90 dark:bg-slate-800 text-slate-900 dark:text-white p-3 pr-10 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <button
              type="button"
              onClick={() => setShowPassword(!showPassword)}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-500 dark:text-slate-400"
            >
              {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
            </button>
          </div>
          {errors.password && <p className="mt-1 text-sm text-red-500">{errors.password}</p>}
        </div>

        <div>
          <label htmlFor="clubName" className="block text-sm font-medium text-slate-700 dark:text-white mb-1">
            Klubbenavn (valgfritt)
          </label>
          <input
            id="clubName"
            name="clubName"
            type="text"
            value={formData.clubName}
            onChange={handleChange}
            className="w-full rounded-lg border border-slate-300 dark:border-slate-700 bg-white/90 dark:bg-slate-800 text-slate-900 dark:text-white p-3 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div>
          <label htmlFor="invitationCode" className="block text-sm font-medium text-slate-700 dark:text-white mb-1">
            Invitasjonskode (valgfritt)
          </label>
          <input
            id="invitationCode"
            name="invitationCode"
            type="text"
            value={formData.invitationCode}
            onChange={handleChange}
            className="w-full rounded-lg border border-slate-300 dark:border-slate-700 bg-white/90 dark:bg-slate-800 text-slate-900 dark:text-white p-3 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <button
          type="submit"
          disabled={loading}
          className="w-full rounded-lg bg-blue-600 hover:bg-blue-700 text-white py-3 font-semibold disabled:opacity-50 transition-colors"
        >
          {loading ? "Registrerer..." : "Registrer"}
        </button>
      </form>
    </div>
  </div>
);
}