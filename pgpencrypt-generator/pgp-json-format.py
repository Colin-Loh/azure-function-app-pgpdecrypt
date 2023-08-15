import json

data = """-----BEGIN PGP PRIVATE KEY BLOCK-----
Version: BCPG C# v1.9.0.0

-----END PGP PRIVATE KEY BLOCK-----
"""

# Assuming you want the PGP data to be stored with the key "pgpKey"
json_data = {
    "pgpKey": data
}

with open("pgp_data.json", "w") as file:
    json.dump(json_data, file, indent=4)
