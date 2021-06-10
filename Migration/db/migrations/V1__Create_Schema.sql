CREATE TABLE `users` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `is_consultant` boolean NOT NULL,
  `name` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `password` varchar(255),
  `cpf_cnpj` varchar(255),
  `short_description` varchar(255),
  `long_description` varchar(255),
  `profile_picture` blob,
  `is_email_confirmed` boolean NOT NULL,
  `email_confirmation_code` binary(16),
  `created_at` datetime NOT NULL,
  `is_admin` boolean NOT NULL DEFAULT false
);

CREATE TABLE `steps` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `type` varchar(255) NOT NULL,
  `display_name` varchar(255) NOT NULL,
  `create_schema` json NOT NULL,
  `submit_schema` json NOT NULL,
  `user_id` bigint
);

CREATE TABLE `services` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `title` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `is_global` boolean NOT NULL,
  `is_deleted` boolean NOT NULL
);

CREATE TABLE `services_steps` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `step_id` bigint NOT NULL,
  `service_id` bigint NOT NULL,
  `order` int NOT NULL,
  `title` varchar(255) NOT NULL,
  `create_data` json NOT NULL
);

CREATE TABLE `appointments` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `service_id` bigint NOT NULL,
  `client_id` bigint NOT NULL,
  `start_date` datetime NOT NULL,
  `end_date` datetime,
  `is_completed` boolean NOT NULL
);

CREATE TABLE `appointment_steps` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `appointment_id` bigint NOT NULL,
  `step_id` bigint NOT NULL,
  `submit_data` json NOT NULL,
  `is_completed` boolean NOT NULL,
  `date_completed` datetime
);

CREATE TABLE `ratings` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `appointment_id` bigint NOT NULL,
  `stars` int NOT NULL,
  `comment` varchar(255)
);

ALTER TABLE `steps` ADD CONSTRAINT `steps_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `services` ADD CONSTRAINT `user_service` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `services_steps` ADD CONSTRAINT `services_steps_steps` FOREIGN KEY (`step_id`) REFERENCES `steps` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `services_steps` ADD CONSTRAINT `services_steps_service` FOREIGN KEY (`service_id`) REFERENCES `services` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `appointments` ADD CONSTRAINT `appointments_service` FOREIGN KEY (`service_id`) REFERENCES `services` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `appointments` ADD CONSTRAINT `appointments_client` FOREIGN KEY (`client_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `appointment_steps` ADD CONSTRAINT `appointment_steps_appointment` FOREIGN KEY (`appointment_id`) REFERENCES `appointments` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `appointment_steps` ADD CONSTRAINT `appointment_steps_steps` FOREIGN KEY (`step_id`) REFERENCES `steps` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `ratings` ADD CONSTRAINT `ratings_appointment` FOREIGN KEY (`appointment_id`) REFERENCES `appointments` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;
