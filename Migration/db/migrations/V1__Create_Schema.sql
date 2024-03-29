CREATE TABLE `users` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `type` ENUM ('client', 'consultant', 'admin') NOT NULL,
  `name` varchar(255) NOT NULL,
  `public_name` varchar(255),
  `email` varchar(255) NOT NULL,
  `password` varchar(255),
  `cpf_cnpj` varchar(255),
  `gender` varchar(255),
  `phone_number` varchar(255),
  `short_description` varchar(255),
  `long_description` varchar(255),
  `profile_picture_id` bigint,
  `is_email_confirmed` boolean NOT NULL,
  `email_confirmation_code` binary(16),
  `created_at` datetime NOT NULL,
  `rate_mean_stars` float NOT NULL DEFAULT 0,
  `rate_count` bigint NOT NULL DEFAULT 0,
  `refresh_token` varchar(500),
  `refresh_token_expiry_time` datetime,
  `login_provider` varchar(255)
);

CREATE TABLE `steps` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `type` varchar(255) NOT NULL,
  `display_name` varchar(255) NOT NULL,
  `create_schema` json NOT NULL,
  `submit_schema` json NOT NULL,
  `user_id` bigint,
  `allow_file_upload` boolean NOT NULL DEFAULT false,
  `target_user` ENUM ('client', 'consultant', 'admin') NOT NULL DEFAULT "client"
);

CREATE TABLE `services` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `title` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `picture_id` bigint,
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
  `appointment_id` bigint UNIQUE NOT NULL,
  `stars` int NOT NULL,
  `comment` varchar(255),
  `created_at` datetime NOT NULL
);

CREATE TABLE `file_content` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `file_guid` binary(16) UNIQUE NOT NULL,
  `content` mediumblob NOT NULL
);

CREATE TABLE `file_details` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `content_id` bigint NOT NULL,
  `guid` binary(16) NOT NULL,
  `name` varchar(255) NOT NULL,
  `type` varchar(255) NOT NULL,
  `size` bigint NOT NULL,
  `uploader_id` bigint NOT NULL
);

CREATE TABLE `appointment_step_files` (
  `id` bigint PRIMARY KEY AUTO_INCREMENT,
  `appointment_step_id` bigint NOT NULL,
  `file_id` bigint NOT NULL
);

ALTER TABLE `users` ADD CONSTRAINT `user_profile_picture` FOREIGN KEY (`profile_picture_id`) REFERENCES `file_details` (`id`) ON DELETE SET NULL ON UPDATE NO ACTION;

ALTER TABLE `steps` ADD CONSTRAINT `steps_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `services` ADD CONSTRAINT `service_picture` FOREIGN KEY (`picture_id`) REFERENCES `file_details` (`id`) ON DELETE SET NULL ON UPDATE NO ACTION;

ALTER TABLE `services` ADD CONSTRAINT `user_service` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `services_steps` ADD CONSTRAINT `services_steps_steps` FOREIGN KEY (`step_id`) REFERENCES `steps` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `services_steps` ADD CONSTRAINT `services_steps_service` FOREIGN KEY (`service_id`) REFERENCES `services` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `appointments` ADD CONSTRAINT `appointments_service` FOREIGN KEY (`service_id`) REFERENCES `services` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `appointments` ADD CONSTRAINT `appointments_client` FOREIGN KEY (`client_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `appointment_steps` ADD CONSTRAINT `appointment_steps_appointment` FOREIGN KEY (`appointment_id`) REFERENCES `appointments` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `appointment_steps` ADD CONSTRAINT `appointment_steps_steps` FOREIGN KEY (`step_id`) REFERENCES `steps` (`id`) ON DELETE RESTRICT ON UPDATE NO ACTION;

ALTER TABLE `ratings` ADD CONSTRAINT `ratings_appointment` FOREIGN KEY (`appointment_id`) REFERENCES `appointments` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `file_details` ADD CONSTRAINT `files_details_content` FOREIGN KEY (`content_id`) REFERENCES `file_content` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `file_details` ADD CONSTRAINT `files_user` FOREIGN KEY (`uploader_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `appointment_step_files` ADD CONSTRAINT `appointment_step_files_appointment` FOREIGN KEY (`appointment_step_id`) REFERENCES `appointment_steps` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

ALTER TABLE `appointment_step_files` ADD CONSTRAINT `appointment_step_files_file` FOREIGN KEY (`file_id`) REFERENCES `file_details` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

CREATE INDEX `guid_index` ON `file_details` (`guid`) USING BTREE;
