CREATE TABLE `users` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `is_consultant` boolean,
  `name` varchar(255),
  `email` varchar(255),
  `password` varchar(255),
  `cpf_cnpj` varchar(255),
  `short_description` varchar(255),
  `long_description` varchar(255),
  `profile_picture` blob,
  `is_email_confirmed` boolean,
  `email_confirmation_code` binary(16),
  `created_at` datetime,
  `refresh_token` varchar(255),
  `refresh_token_expiry_time` datetime
);

CREATE TABLE `steps` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `type` varchar(255),
  `display_name` varchar(255),
  `create_schema` varchar(255),
  `submit_schema` varchar(255)
);

CREATE TABLE `services` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `user_id` bigint,
  `title` varchar(255),
  `description` varchar(255),
  `is_global` boolean,
  `is_deleted` boolean
);

CREATE TABLE `services_steps` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `step_id` bigint,
  `service_id` bigint,
  `order` int,
  `title` varchar(255),
  `create_data` varchar(255)
);

CREATE TABLE `appointments` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `service_id` bigint,
  `client_id` bigint,
  `start_date` datetime,
  `end_date` datetime
);

CREATE TABLE `appointment_steps` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `appointment_id` bigint,
  `step_id` bigint,
  `submit_data` varchar(255),
  `is_completed` varchar(255),
  `date_completed` datetime
);

ALTER TABLE `services` ADD FOREIGN KEY (`user_id`) REFERENCES `users` (`id`);

ALTER TABLE `services_steps` ADD FOREIGN KEY (`step_id`) REFERENCES `steps` (`id`);

ALTER TABLE `services_steps` ADD FOREIGN KEY (`service_id`) REFERENCES `services` (`id`);

ALTER TABLE `appointments` ADD FOREIGN KEY (`service_id`) REFERENCES `services` (`id`);

ALTER TABLE `appointments` ADD FOREIGN KEY (`client_id`) REFERENCES `users` (`id`);

ALTER TABLE `appointment_steps` ADD FOREIGN KEY (`appointment_id`) REFERENCES `services` (`id`);

ALTER TABLE `appointment_steps` ADD FOREIGN KEY (`step_id`) REFERENCES `steps` (`id`);
