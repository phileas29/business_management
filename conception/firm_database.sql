CREATE TABLE "f_client" (
  "c_id" SERIAL PRIMARY KEY NOT NULL,
  "c_fk_media_id" INT,
  "c_fk_city_id" INT NOT NULL,
  "c_fk_birth_city_id" INT,
  "c_name" VARCHAR NOT NULL,
  "c_firstname" VARCHAR NOT NULL,
  "c_address" VARCHAR,
  "c_email" VARCHAR,
  "c_phone_fixed" VARCHAR,
  "c_phone_cell" VARCHAR,
  "c_is_pro" bit NOT NULL,
  "c_location_long" decimal(9,7),
  "c_location_lat" decimal(9,7),
  "c_distance" decimal(3,1),
  "c_travel_time" INT,
  "c_urssaf_uuid" VARCHAR,
  "c_is_man" bit,
  "c_birth_name" VARCHAR,
  "c_birth_country_code" INT,
  "c_birth_date" date,
  "c_bic" VARCHAR,
  "c_iban" VARCHAR,
  "c_account_holder" VARCHAR
);

CREATE TABLE "f_intervention" (
  "i_id" SERIAL PRIMARY KEY NOT NULL,
  "i_fk_client_id" INT NOT NULL,
  "i_fk_invoice_id" INT,
  "i_fk_category_id" INT NOT NULL,
  "i_date" date NOT NULL,
  "i_description" VARCHAR,
  "i_nb_round_trip" INT NOT NULL
);

CREATE TABLE "f_invoice" (
  "in_id" SERIAL PRIMARY KEY NOT NULL,
  "in_fk_payment_id" INT NOT NULL,
  "in_invoice_date" date NOT NULL,
  "in_receipt_date" date,
  "in_credit_date" date,
  "in_amount" INT NOT NULL,
  "in_is_eligible_deferred_tax_credit" bit,
  "in_urssaf_payment_request_uuid" VARCHAR
);

CREATE TABLE "f_city" (
  "ci_id" SERIAL PRIMARY KEY NOT NULL,
  "ci_postal_code" VARCHAR NOT NULL,
  "ci_name" VARCHAR NOT NULL,
  "ci_insee_code" INT,
  "ci_depart_code" INT
);

CREATE TABLE "f_purchase" (
  "p_id" SERIAL PRIMARY KEY NOT NULL,
  "p_fk_payment_id" INT NOT NULL,
  "p_fk_category_id" INT NOT NULL,
  "p_fk_supplier_id" INT NOT NULL,
  "p_invoice_date" date,
  "p_disbursement_date" date NOT NULL,
  "p_debit_date" date,
  "p_description" VARCHAR,
  "p_amount" decimal(6,2) NOT NULL
);

CREATE TABLE "f_category" (
  "ca_id" SERIAL PRIMARY KEY NOT NULL,
  "ca_fk_category_type_id" INT NOT NULL,
  "ca_name" VARCHAR NOT NULL
);

CREATE TABLE "f_category_type" (
  "ct_id" SERIAL PRIMARY KEY NOT NULL,
  "ct_name" VARCHAR NOT NULL
);

ALTER TABLE "f_client" ADD FOREIGN KEY ("c_fk_city_id") REFERENCES "f_city" ("ci_id");

ALTER TABLE "f_client" ADD FOREIGN KEY ("c_fk_media_id") REFERENCES "f_category" ("ca_id");

ALTER TABLE "f_intervention" ADD FOREIGN KEY ("i_fk_client_id") REFERENCES "f_client" ("c_id");

ALTER TABLE "f_intervention" ADD FOREIGN KEY ("i_fk_invoice_id") REFERENCES "f_invoice" ("in_id");

ALTER TABLE "f_intervention" ADD FOREIGN KEY ("i_fk_category_id") REFERENCES "f_category" ("ca_id");

ALTER TABLE "f_invoice" ADD FOREIGN KEY ("in_fk_payment_id") REFERENCES "f_category" ("ca_id");

ALTER TABLE "f_purchase" ADD FOREIGN KEY ("p_fk_category_id") REFERENCES "f_category" ("ca_id");

ALTER TABLE "f_purchase" ADD FOREIGN KEY ("p_fk_payment_id") REFERENCES "f_category" ("ca_id");

ALTER TABLE "f_purchase" ADD FOREIGN KEY ("p_fk_supplier_id") REFERENCES "f_category" ("ca_id");

ALTER TABLE "f_category" ADD FOREIGN KEY ("ca_fk_category_type_id") REFERENCES "f_category_type" ("ct_id");

ALTER TABLE "f_client" ADD FOREIGN KEY ("c_fk_birth_city_id") REFERENCES "f_city" ("ci_id");
