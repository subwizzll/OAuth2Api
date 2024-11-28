## OAuth2 API

### Local Development Setup

Run the appropriate setup script for your operating system. This will set up your environment variables and create necessary symbolic links.

**Unix/MacOS:**
```bash
./setup.sh
```

**Windows:**
```powershell
.\setup.ps1
```

### Required Environment Variables
Make sure your `.env.local` file contains the following variables:
- `JWT_KEY`: Your JWT signing key
- `JWT_ISSUER`: JWT issuer
- `JWT_AUDIENCE`: JWT audience
- `CONNECTION_STRING`: Database connection string