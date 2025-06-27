"use client";

import { useEffect, useState } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import axios from "axios";
import { CheckCircle, XCircle, Loader2 } from "lucide-react";
import Image from "next/image";
import logo from "@/public/logo.svg"; // valgfritt logo

export default function EmailConfirm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [status, setStatus] = useState<"pending" | "success" | "error">("pending");

  useEffect(() => {
    const userId = searchParams.get("userId");
    const token = searchParams.get("token");

    if (!userId || !token) {
      setStatus("error");
      return;
    }

    axios
      .post("http://localhost:5227/api/Auth/confirm-email", { userId, token })
      .then(() => {
        setStatus("success");
        setTimeout(() => router.push("/login"), 4000);
      })
      .catch(() => {
        setStatus("error");
        setTimeout(() => router.push("/login"), 4000);
      });
  }, [searchParams, router]);

  return (
    <div className="min-h-screen bg-gradient-to-tr from-gray-50 to-slate-100 flex items-center justify-center px-6">
      <div className="bg-white rounded-3xl shadow-xl p-10 max-w-md w-full text-center animate-fade-in">
        {/* Optional logo */}
        {/* <Image src={logo} alt="Logo" className="mx-auto mb-6" width={50} /> */}

        {status === "pending" && (
          <>
            <Loader2 className="w-12 h-12 text-blue-500 animate-spin mx-auto mb-4" />
            <h1 className="text-lg font-semibold text-gray-700">
              Bekrefter e-posten din...
            </h1>
          </>
        )}

        {status === "success" && (
          <>
            <CheckCircle className="w-16 h-16 text-green-500 mx-auto mb-4" />
            <h1 className="text-2xl font-bold text-green-700">
              E-post bekreftet
            </h1>
            <p className="text-sm text-gray-600 mt-2">
              Du blir snart sendt til innlogging.
            </p>
          </>
        )}

        {status === "error" && (
          <>
            <XCircle className="w-16 h-16 text-red-500 mx-auto mb-4" />
            <h1 className="text-2xl font-bold text-red-700">
              Bekreftelse mislyktes
            </h1>
            <p className="text-sm text-gray-500 mt-2">
              Noe gikk galt. Prøv igjen senere.
            </p>
          </>
        )}
      </div>
    </div>
  );
}
