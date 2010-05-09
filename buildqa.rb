puts "What version are you building? (x.x.x default 0.0.0)"
version = gets.chomp
version = "0.0.0" unless version =~ /^\d\.\d\.\d$/

puts "Do you want to deploy to QA (y/n, default n)"
do_deploy = gets.chomp.downcase
deploy =  do_deploy == 'y' ? "deploy" : ""

puts "Enter any additional arguments you want passed to the nant process(-D:property=value -D:otherprop=whatever etc...)"
nant_args = gets.chomp

cmd = "nant /f:release/buildfiles/default.build -D:version=#{version} #{nant_args} build.svn package #{deploy}"
puts "Running: #{cmd}"

out = IO.popen(cmd);

while (line = out.gets)
  puts line
end

gets

