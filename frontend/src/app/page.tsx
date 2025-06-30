"use client";

import Layout from '../components/Layout';
import HeroSection from "../components/home/HeroSection";
import FeaturesSection from '@/components/home/FeaturesSection';
import ScreenshotsSection from '@/components/home/ScreenshotsSection';
import CommunicationSection from '@/components/home/CommunicationSection';
import CTASection from '@/components/home/CTASection';

export default function Home() {
  return (
    <Layout>
     <HeroSection/>
     <FeaturesSection/>
     <ScreenshotsSection/>
     <CommunicationSection/>
     <CTASection/>
    </Layout>
  );
}
