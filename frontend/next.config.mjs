/** @type {import('next').NextConfig} */
const nextConfig = {
  typescript: {
    ignoreBuildErrors: true,
  },
  images: {
    unoptimized: true,
  },
  async rewrites() {
    return [
      {
        source: "/backend-api/:path*",
        destination: "http://localhost:5258/api/:path*",
      },
    ]
  },
}

export default nextConfig
